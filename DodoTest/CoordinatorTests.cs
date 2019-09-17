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
	public async Task AddCoordinatorNeed()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = await DodoServer.TelegramGateway.FakeMessage("NEED", user.TelegramUser);
		msg = await DodoServer.TelegramGateway.FakeMessage("7/10 08:00", user.TelegramUser);
		msg = await DodoServer.TelegramGateway.FakeMessage("3", user.TelegramUser);

		var need = DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Single();
		Assert.IsTrue(need.WorkingGroup.Name == user.CoordinatorRoles.Single().WorkingGroup.Name);
		Assert.IsTrue(need.TimeNeeded == new DateTime(2019, 10, 7, 8, 0, 0));
		Assert.IsTrue(need.Amount == 3);
	}

	[TestMethod]
	public async Task AddCoordinatorNeed_Shortcode()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = await DodoServer.TelegramGateway.FakeMessage("NEED 7/10 08:00 3", user.TelegramUser);

		var need = DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Single();
		Assert.IsTrue(need.WorkingGroup.Name == user.CoordinatorRoles.Single().WorkingGroup.Name);
		Assert.IsTrue(need.TimeNeeded == new DateTime(2019, 10, 7, 8, 0, 0));
		Assert.IsTrue(need.Amount == 3);
	}

	[TestMethod]
	public async Task RemoveCoordinatorNeed()
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

		var msg = await DodoServer.TelegramGateway.FakeMessage("DELETENEED", user.TelegramUser);
		msg = await DodoServer.TelegramGateway.FakeMessage("0", user.TelegramUser);
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
		var msg = await DodoServer.TelegramGateway.FakeMessage("DELETENEED", user.TelegramUser);
		Assert.IsFalse(msg.Content.Contains("Sorry"));
		Assert.IsTrue(DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Count == 0);
	}

	[TestMethod]
	public async Task CheckCoordinatorWhoIs()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = await DodoServer.TelegramGateway.FakeMessage("WHOIS", user.TelegramUser);
		msg = await DodoServer.TelegramGateway.FakeMessage(user.CoordinatorRoles.Single().WorkingGroup.ShortCode, user.TelegramUser);
		Assert.IsTrue(msg.Content.Contains(user.PhoneNumber));
	}

	[TestMethod]
	public async Task CheckCoordinatorWhoIs_Shortcode()
	{
		var user = GetTestUser(EUserAccessLevel.Coordinator);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = await DodoServer.TelegramGateway.FakeMessage("WHOIS " + user.CoordinatorRoles.Single().WorkingGroup.ShortCode, user.TelegramUser);
		Assert.IsTrue(msg.Content.Contains(user.PhoneNumber));
	}
}
