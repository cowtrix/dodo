using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using XR.Dodo;
using System.Linq;

namespace DodoTest
{
	[TestClass]
	public class BasicMessageTests : TestBase
	{
		[TestMethod]
		public async Task CheckCoordinator()
		{
			//Assert.IsFalse(DodoServer.SessionManager.IsCoordinator("04472782157"));
			//Assert.IsTrue(DodoServer.SessionManager.IsCoordinator("07960078593"));
		}

		[TestMethod]
		public void ValidatePhoneNumbers()
		{
			var invalidNumber = "Joe Blocks";
			Assert.IsTrue(!PhoneExtensions.ValidateNumber(ref invalidNumber));
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
		}
	}
}
