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
			var wg = new WorkingGroup("Test", EParentGroup.MovementSupport, "Test role", 3);
			DodoServer.SiteManager.GetSite(3).WorkingGroups.Add(wg);
			var user = new User()
			{
				Name = "Test",
				PhoneNumber = ph,
				TelegramUser = telegramID,
				CoordinatorRoles = new System.Collections.Generic.HashSet<WorkingGroup>()
				{
					wg
				}
			};
			var session = DodoServer.SessionManager.GetOrCreateSession(user);
			var msg = await DodoServer.TelegramGateway.FakeMessage("NEED", user.TelegramUser);
			msg = await DodoServer.TelegramGateway.FakeMessage("7/10 08:00", user.TelegramUser);
			msg = await DodoServer.TelegramGateway.FakeMessage("3", user.TelegramUser);

			var need = DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Single();
			Assert.IsTrue(need.WorkingGroup.Name == wg.Name);
			Assert.IsTrue(need.TimeNeeded == new DateTime(2019, 10, 7, 8, 0, 0));
			Assert.IsTrue(need.Amount == 3);
		}

		[TestMethod]
		public async Task CheckCoordinatorWhoIs()
		{
			var ph = "441315103992";
			var telegramID = 997875;
			var wg = new WorkingGroup("Test", EParentGroup.MovementSupport, "Test role", 3);
			DodoServer.SiteManager.GetSite(3).WorkingGroups.Add(wg);
			var user = new User()
			{
				Name = "Test",
				PhoneNumber = ph,
				TelegramUser = telegramID,
				CoordinatorRoles = new System.Collections.Generic.HashSet<WorkingGroup>()
				{
					wg
				}
			};
			var session = DodoServer.SessionManager.GetOrCreateSession(user);

			var msg = await DodoServer.TelegramGateway.FakeMessage("WHOIS TE", user.TelegramUser);
		}
	}
}
