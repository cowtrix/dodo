using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Linq;
using XR.Dodo;

namespace DodoTest
{
	[TestClass]
	public class BasicMessageTests : TestBase
	{
		/*[TestMethod]
		public async Task CheckCoordinator()
		{
			//Assert.IsFalse(DodoServer.SessionManager.IsCoordinator("04472782157"));
			//Assert.IsTrue(DodoServer.SessionManager.IsCoordinator("07960078593"));
		}

		[TestMethod]
		public void ValidatePhoneNumbers()
		{
			var invalidNumber = "Joe Blocks";
			Assert.IsTrue(!ValidationExtensions.ValidateNumber(ref invalidNumber));
		}

		[TestMethod]
		public async Task NewVolunteerWorkflow()
		{
			var rnd = new Random();
			var ph = $"44{rnd.Next(100_000_000, 999_999_999)}";
			var response = await m_simulator.SendSMS(ph, "REGISTER");
			var responseParse = JObject.Parse(response);
			var messages = responseParse.SelectTokens("payload.messages").ToList();
			foreach(var message in messages)
			{
				var innerMessage = message.SelectToken("[0].message");
				TestContext.WriteLine($"Received SMS Response: {innerMessage}");
				Assert.AreEqual("Thank you for registering!", innerMessage.ToString());
			}
		}*/

		[TestMethod]
		public async Task CheckVerification()
		{
			var telegramUser = 999999;
			var response = await DodoServer.TelegramGateway.FakeMessage("VERIFY", telegramUser);
			var code = response.Content.Substring(
				"Please take a moment to verify your phone number. You can do this by texting ".Length, 5);

			var rnd = new Random();
			var ph = "441315103992";
			ValidationExtensions.ValidateNumber(ref ph);
			var smsResponse = await m_simulator.SendSMS(ph, code);

			var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(ph);
			Assert.IsTrue(user.TelegramUser == telegramUser);
			Assert.IsTrue(user.PhoneNumber == ph);
			Assert.IsTrue(user.IsVerified());
		}
	}
}
