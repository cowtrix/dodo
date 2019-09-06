﻿using System;
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
			Assert.IsFalse(DodoServer.SiteManager.IsCoordinator("04472782157"));
			Assert.IsTrue(DodoServer.SiteManager.IsCoordinator("07960078593"));
		}

		[TestMethod]
		public async Task NewVolunteerWorkflow()
		{
			var response = await m_simulator.SendSMS("REGISTER");
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
