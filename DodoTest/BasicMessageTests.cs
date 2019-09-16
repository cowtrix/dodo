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

		[TestMethod]
		public async Task CheckCoordinatorNeedsWorkflow()
		{
			var ph = "441315103992";
			var telegramID = 997875;
			var user = new User()
			{
				Name = "Test",
				PhoneNumber = ph,
				TelegramUser = telegramID,
				CoordinatorRoles = new System.Collections.Generic.HashSet<WorkingGroup>()
				{
					new WorkingGroup("Disability Coord", EParentGroup.ActionSupport, "", 4),
				}
			};
			var session = DodoServer.SessionManager.GetOrCreateSession(user);

			var msg = await DodoServer.TelegramGateway.FakeMessage("NEED", user.TelegramUser);
			Assert.IsTrue(msg.Content.Contains("Disability Coord"));
		}


	}
}
