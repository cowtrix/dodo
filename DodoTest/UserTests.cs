using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XR.Dodo;
using DodoTest;

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

		var response = await DodoServer.TelegramGateway.FakeMessage("VERIFY", initialUser.TelegramUser);	// Verify process
		var code = response.Content.Substring(
			"Please take a moment to verify your phone number. You can do this by texting ".Length, 5);	// Extract verification code

		var ph = coord.PhoneNumber;
		ValidationExtensions.ValidateNumber(ref ph);
		var smsResponse = await m_simulator.SendSMS(ph, code, Phone.ESMSMode.Verification);	// Coord sms' code to number

		var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(ph);    // So the user here should be the coord
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

		var response = await DodoServer.TelegramGateway.FakeMessage("VERIFY", initialUser.TelegramUser);
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
		var response = await DodoServer.TelegramGateway.FakeMessage("NEED", user.TelegramUser);
		Assert.IsTrue(response.Content.Contains("HELP"));
	}

	[TestMethod]
	public async Task VolunteerCannotRequestWhoIs()
	{
		var user = GetTestUser(EUserAccessLevel.Volunteer);
		var response = await DodoServer.TelegramGateway.FakeMessage("WHOIS", user.TelegramUser);
		Assert.IsTrue(response.Content.Contains("HELP"));
	}
}
