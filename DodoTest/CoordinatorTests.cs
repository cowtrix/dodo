using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Linq;
using XR.Dodo;
using DodoTest;

[TestClass]
public class CoordinatorTests : TestBase
{
	[TestMethod]
	public void CannotAddMoreThanMaxNeeds()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		for (var i = 0; i < CoordinatorNeedsManager.MaxNeedCountPerWorkingGroup + 1; ++i)
		{
			var msg = DodoServer.TelegramGateway.FakeMessage("NEED", user.TelegramUser);
			msg = DodoServer.TelegramGateway.FakeMessage("7/10 08:00", user.TelegramUser);
			msg = DodoServer.TelegramGateway.FakeMessage("3", user.TelegramUser);
			msg = DodoServer.TelegramGateway.FakeMessage("tests", user.TelegramUser);
		}
		var role = user.CoordinatorRoles.First();
		Assert.IsTrue(DodoServer.CoordinatorNeedsManager.GetNeedsForWorkingGroup(role.SiteCode, role.WorkingGroup).Count()
			== CoordinatorNeedsManager.MaxNeedCountPerWorkingGroup);
	}

	[TestMethod]
	public async Task AddNeed_Case1()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("NEED", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("7/10 08:00", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("3", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("TEST", user.TelegramUser);

		var need = DodoServer.CoordinatorNeedsManager.Data.CurrentNeeds.Single().Value;
		Assert.IsTrue(need.WorkingGroupCode == user.CoordinatorRoles.Single().WorkingGroupCode);
		Assert.IsTrue(need.TimeNeeded == new DateTime(2019, 10, 7, 8, 0, 0));
		Assert.IsTrue(need.SiteCode == user.CoordinatorRoles.Single().SiteCode);
		Assert.IsTrue(need.Amount == 3);
		Assert.IsTrue(need.Description == "TEST");
	}

	[TestMethod]
	public async Task AddNeed_Shortcode1()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("NEED 7/10 08:00 3", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("test", user.TelegramUser);

		var need = DodoServer.CoordinatorNeedsManager.Data.CurrentNeeds.Single().Value;
		Assert.IsTrue(need.WorkingGroupCode == user.CoordinatorRoles.Single().WorkingGroupCode);
		Assert.IsTrue(need.TimeNeeded == new DateTime(2019, 10, 7, 8, 0, 0));
		Assert.IsTrue(need.SiteCode == user.CoordinatorRoles.Single().SiteCode);
		Assert.IsTrue(need.Amount == 3);
		Assert.AreEqual(need.Description, "test");
	}

	[TestMethod]
	public async Task RemoveNeedFromMany()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var role = user.CoordinatorRoles.First();
		DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, role.WorkingGroup, role.SiteCode, 4, DateTime.Now, "test 1");
		DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, role.WorkingGroup, role.SiteCode, 6, DateTime.Now, "test 2");

		var msg = DodoServer.TelegramGateway.FakeMessage("DELETENEED", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage(DodoServer.CoordinatorNeedsManager.Data.CurrentNeeds.Keys.Random(),
			user.TelegramUser);
		Assert.IsFalse(msg.Content.Contains("Sorry"));
		Assert.IsTrue(DodoServer.CoordinatorNeedsManager.Data.CurrentNeeds.Count == 1);
	}

	[TestMethod]
	public async Task RemoveSingleNeed()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var role = user.CoordinatorRoles.First();
		Assert.IsTrue(DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, role.WorkingGroup, role.SiteCode, 6, DateTime.Now + TimeSpan.FromDays(1), "test 2"));
		var need = DodoServer.CoordinatorNeedsManager.Data.CurrentNeeds.Single();
		Assert.IsTrue(DodoServer.CoordinatorNeedsManager.Data.CurrentNeeds.Count == 1);
		var msg = DodoServer.TelegramGateway.FakeMessage("DELETENEED", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage(need.Key, user.TelegramUser);
		Assert.IsTrue(DodoServer.CoordinatorNeedsManager.Data.CurrentNeeds.Count == 0);
	}

	[TestMethod]
	public async Task CheckWhoIs()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("WHOIS", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage(user.CoordinatorRoles.Single().WorkingGroup.ShortCode, user.TelegramUser);
		Assert.IsTrue(msg.Content.Contains(user.PhoneNumber));
	}

	[TestMethod]
	public async Task CheckWhoIs_Shortcode()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("WHOIS " + user.CoordinatorRoles.Single().WorkingGroup.ShortCode, user.TelegramUser);
		Assert.IsTrue(msg.Content.Contains(user.PhoneNumber));
	}
}
