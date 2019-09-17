using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XR.Dodo;
using DodoTest;

[TestClass]
public class UserTests : TestBase
{
	[TestMethod]
	public async Task CheckVerification()
	{
		var telegramUser = 999999;
		var response = await DodoServer.TelegramGateway.FakeMessage("VERIFY", telegramUser);
		var code = response.Content.Substring(
			"Please take a moment to verify your phone number. You can do this by texting ".Length, 5);

		var ph = "441315103992";
		ValidationExtensions.ValidateNumber(ref ph);
		var smsResponse = await m_simulator.SendSMS(ph, code);

		var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(ph);
		Assert.IsTrue(user.TelegramUser == telegramUser);
		Assert.IsTrue(user.PhoneNumber == ph);
		Assert.IsTrue(user.IsVerified());
	}
}
