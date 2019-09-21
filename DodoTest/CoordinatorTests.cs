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
	public async Task AddNeed_Case1()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("NEED", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("7/10 08:00", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("3", user.TelegramUser);

		var need = DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Single();
		Assert.IsTrue(need.WorkingGroup.Name == user.CoordinatorRoles.Single().WorkingGroup.Name);
		Assert.IsTrue(need.TimeNeeded == new DateTime(2019, 10, 7, 8, 0, 0));
		Assert.IsTrue(need.SiteCode == user.CoordinatorRoles.Single().SiteCode);
		Assert.IsTrue(need.Amount == 3);
	}

	[TestMethod]
	public async Task AddNeed_Shortcode1()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("NEED 7/10 08:00 3", user.TelegramUser);

		var need = DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Single();
		Assert.IsTrue(need.WorkingGroup.Name == user.CoordinatorRoles.Single().WorkingGroup.Name);
		Assert.IsTrue(need.TimeNeeded == new DateTime(2019, 10, 7, 8, 0, 0));
		Assert.IsTrue(need.SiteCode == user.CoordinatorRoles.Single().SiteCode);
		Assert.IsTrue(need.Amount == 3);
	}

	[TestMethod]
	public async Task RemoveNeedFromMany()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var role = user.CoordinatorRoles.First();
		var need1 = new CoordinatorNeedsManager.Need()
		{
			WorkingGroup = role.WorkingGroup,
			SiteCode = role.SiteCode,
			TimeNeeded = DateTime.Now + TimeSpan.FromDays(1),
			TimeOfRequest = DateTime.Now,
			Amount = 4,
		};
		var need2 = new CoordinatorNeedsManager.Need()
		{
			WorkingGroup = role.WorkingGroup,
			SiteCode = role.SiteCode,
			TimeNeeded = DateTime.Now + TimeSpan.FromDays(2),
			TimeOfRequest = DateTime.Now,
			Amount = 4,
		};
		DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, need1);
		DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, need2);

		var msg = DodoServer.TelegramGateway.FakeMessage("DELETENEED", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("0", user.TelegramUser);
		Assert.IsFalse(msg.Content.Contains("Sorry"));
		Assert.IsTrue(DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Count == 1);
	}

	[TestMethod]
	public async Task RemoveSingleNeed()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var role = user.CoordinatorRoles.First();
		var need1 = new CoordinatorNeedsManager.Need()
		{
			WorkingGroup = role.WorkingGroup,
			SiteCode = role.SiteCode,
			TimeNeeded = DateTime.Now + TimeSpan.FromDays(1),
			TimeOfRequest = DateTime.Now,
			Amount = 4,
		};
		DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, need1);
		Assert.IsTrue(DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Count == 1);
		var msg = DodoServer.TelegramGateway.FakeMessage("DELETENEED", user.TelegramUser);
		Assert.IsFalse(msg.Content.Contains("Sorry"));
		Assert.IsTrue(DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Count == 0);
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
