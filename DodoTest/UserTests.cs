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
		var initialUser = GetTestUser(EUserAccessLevel.Volunteer);
		initialUser.TelegramUser = 99999;
		var response = await DodoServer.TelegramGateway.FakeMessage("VERIFY", initialUser.TelegramUser);
		var code = response.Content.Substring(
			"Please take a moment to verify your phone number. You can do this by texting ".Length, 5);

		var ph = "441315103992";
		ValidationExtensions.ValidateNumber(ref ph);
		var smsResponse = await m_simulator.SendSMS(ph, code);

		var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(ph);
		Assert.IsTrue(user.TelegramUser == initialUser.TelegramUser);
		Assert.IsTrue(user.PhoneNumber == ph);
		Assert.IsTrue(user.IsVerified());
	}

	[TestMethod]
	public async Task CheckVolunteerVerification()
	{
		var initialUser = GetTestUser(EUserAccessLevel.Volunteer);
		initialUser.TelegramUser = 99999;
		var response = await DodoServer.TelegramGateway.FakeMessage("VERIFY", initialUser.TelegramUser);
		var code = response.Content.Substring(
			"Please take a moment to verify your phone number. You can do this by texting ".Length, 5);

		var ph = "441315103992";
		ValidationExtensions.ValidateNumber(ref ph);
		var smsResponse = await m_simulator.SendSMS(ph, code);

		var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(ph);
		Assert.IsTrue(user.TelegramUser == initialUser.TelegramUser);
		Assert.IsTrue(user.PhoneNumber == ph);
		Assert.IsTrue(user.IsVerified());
	}

	[TestMethod]
	public async Task VolunteerCannotRequestNeed()
	{
		var user = GetTestUser(EUserAccessLevel.Volunteer);
		user.TelegramUser = 99999;
		var response = await DodoServer.TelegramGateway.FakeMessage("NEED", user.TelegramUser);
		Assert.IsTrue(response.Content.Contains("HELP"));
	}

	[TestMethod]
	public async Task VolunteerCannotRequestWhoIs()
	{
		var user = GetTestUser(EUserAccessLevel.Volunteer);
		user.TelegramUser = 99999;
		var response = await DodoServer.TelegramGateway.FakeMessage("WHOIS", user.TelegramUser);
		Assert.IsTrue(response.Content.Contains("HELP"));
	}
}
