using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XR.Dodo;
using DodoTest;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

[TestClass]
public class UserTests : TestBase
{
	[TestMethod]
	public async Task CheckCoordinatorVerification()
	{
		var coord = GetTestUser(EUserAccessLevel.Coordinator);  // Coord account has been loaded from RSO spreadsheet, has a phone number
		coord.Email = "test@test.com";
		coord.TelegramUser = -1;

		var initialUser = GetTestUser(EUserAccessLevel.Volunteer);  // Coord has talked to telegram but system doesn't know they are coord yet
		initialUser.PhoneNumber = null;

		var response = DodoServer.TelegramGateway.FakeMessage("VERIFY", initialUser.TelegramUser);	// Verify process
		var code = response.Content.Substring(
			"Please take a moment to verify your phone number. You can do this by texting ".Length, 5);	// Extract verification code

		var ph = coord.PhoneNumber;
		ValidationExtensions.ValidateNumber(ref ph);
		var smsResponse = await m_simulator.SendSMS(ph, code, Phone.ESMSMode.Verification);	// Coord sms' code to number

		var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(ph);	// So the user here should be the coord
		Assert.IsTrue(ReferenceEquals(initialUser, user));
		Assert.IsTrue(coord.Email == user.Email);
		Assert.IsTrue(coord.PhoneNumber == user.PhoneNumber);
		Assert.IsTrue(user.IsVerified());
		Assert.IsTrue(user.AccessLevel == EUserAccessLevel.Coordinator);
	}

	[TestMethod]
	public async Task CheckVolunteerVerification()
	{
		var initialUser = GetTestUser(EUserAccessLevel.Volunteer);
		initialUser.PhoneNumber = null;

		var response = DodoServer.TelegramGateway.FakeMessage("VERIFY", initialUser.TelegramUser);
		var code = response.Content.Substring(
			"Please take a moment to verify your phone number. You can do this by texting ".Length, 5);

		var ph = "441315103992";
		ValidationExtensions.ValidateNumber(ref ph);
		var smsResponse = await m_simulator.SendSMS(ph, code, Phone.ESMSMode.Verification);

		var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(ph);
		Assert.IsTrue(user.TelegramUser == initialUser.TelegramUser);
		Assert.IsTrue(user.PhoneNumber == ph);
		Assert.IsTrue(user.IsVerified());
		Assert.IsTrue(user.AccessLevel == EUserAccessLevel.Volunteer);
	}

	[TestMethod]
	public async Task VolunteerCannotRequestNeed()
	{
		var user = GetTestUser(EUserAccessLevel.Volunteer);
		var response = DodoServer.TelegramGateway.FakeMessage("NEED", user.TelegramUser);
		Assert.IsTrue(response.Content.Contains("Sorry"));
	}

	[TestMethod]
	public async Task VolunteerCannotRequestWhoIs()
	{
		var user = GetTestUser(EUserAccessLevel.Volunteer);
		var response = DodoServer.TelegramGateway.FakeMessage("WHOIS", user.TelegramUser);
		Assert.IsTrue(response.Content.Contains("Sorry"));
	}

	[TestMethod]
	public async Task VolunteersAreContacted()
	{
		var volunteers = new ConcurrentDictionary<User, bool>();
		string code = null;
		void VolunteerReceiveRequest(ServerMessage mesg, UserSession sess)
		{
			var user = sess.GetUser();
			Assert.IsTrue(volunteers.ContainsKey(user));
			Assert.IsFalse(volunteers[user]);
			Assert.IsTrue(mesg.Content.Contains(code));
			volunteers[sess.GetUser()] = true;
		};
		for(var i = 0; i < 10; ++i)
		{
			var vol = GetTestUser(EUserAccessLevel.Volunteer);
			vol.OnMsgReceived += VolunteerReceiveRequest;
			volunteers.TryAdd(vol, false);
		}

		var requester = GetTestUser(EUserAccessLevel.Coordinator);
		var msg = DodoServer.TelegramGateway.FakeMessage("NEED", requester.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("7/10 08:00", requester.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("10", requester.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("tests", requester.TelegramUser);
		code = DodoServer.CoordinatorNeedsManager.CurrentNeeds.Single().Key;

		while (volunteers.Any(x => !x.Value)) ;

		bool confirmed = false;
		void ReceiveVolunteerNumbers(ServerMessage mesg, UserSession sess)
		{
			if(mesg.Content.Contains("This request is now complete and has been removed."))
			{
				confirmed = true;
			}
			var user = volunteers.Single(x => mesg.Content.Contains(x.Key.PhoneNumber)).Key;
			volunteers[user] = false;
		};
		requester.OnMsgReceived += ReceiveVolunteerNumbers;


		foreach (var vol in volunteers)
		{
			vol.Key.OnMsgReceived = null;
			msg = DodoServer.TelegramGateway.FakeMessage(code, vol.Key.TelegramUser);
		}

		while (volunteers.Any(x => x.Value) || !confirmed) ;

		requester.OnMsgReceived = null;
		DodoServer.CoordinatorNeedsManager.ProcessNeeds();
		Assert.IsTrue(!DodoServer.CoordinatorNeedsManager.CurrentNeeds.Any());
	}
}
