﻿using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using XR.Dodo;
using DodoTest;

[TestClass]
public class RSOCoordinatorTests : TestBase
{
	[TestMethod]
	public async Task AddNeed_Case1()
	{
		var user = GetTestUser(EUserAccessLevel.RSO);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("NEED", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("3", user.TelegramUser);	// Site
		msg = DodoServer.TelegramGateway.FakeMessage("0", user.TelegramUser);	// Parent group
		msg = DodoServer.TelegramGateway.FakeMessage("AD", user.TelegramUser);	// Working group
		msg = DodoServer.TelegramGateway.FakeMessage("02/10 08:00", user.TelegramUser);	// Time
		msg = DodoServer.TelegramGateway.FakeMessage("6", user.TelegramUser);	// Amount

		var need = DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Single();
		Assert.IsTrue(need.WorkingGroup.ShortCode == "AD");
		Assert.IsTrue(need.SiteCode == 3);
		Assert.IsTrue(need.TimeNeeded == new DateTime(2019, 10, 2, 8, 0, 0));
		Assert.IsTrue(need.Amount == 6);
	}

	[TestMethod]
	public async Task AddNeed_Case2()
	{
		var user = GetTestUser(EUserAccessLevel.RSO);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("need", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("3", user.TelegramUser); // Site
		msg = DodoServer.TelegramGateway.FakeMessage("sd", user.TelegramUser);    // Working group
		msg = DodoServer.TelegramGateway.FakeMessage("20/10 22:00", user.TelegramUser);   // Time
		msg = DodoServer.TelegramGateway.FakeMessage("Many", user.TelegramUser); // Amount

		var need = DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Single();
		Assert.IsTrue(need.WorkingGroup.ShortCode == "SD");
		Assert.IsTrue(need.SiteCode == 3);
		Assert.IsTrue(need.TimeNeeded == new DateTime(2019, 10, 20, 22, 0, 0));
		Assert.IsTrue(need.Amount == int.MaxValue);
	}

	[TestMethod]
	public async Task AddNeed_FailCase1()
	{
		var user = GetTestUser(EUserAccessLevel.RSO);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("need", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("people", user.TelegramUser);

	}

	[TestMethod]
	public async Task AddNeed_Shortcode1()
	{
		var user = GetTestUser(EUserAccessLevel.RSO);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage($"NEED 4 AD 7/10 08:00 4", user.TelegramUser);

		var need = DodoServer.CoordinatorNeedsManager.GetCurrentNeeds().Single();
		Assert.IsTrue(need.WorkingGroup.ShortCode == "AD");
		Assert.IsTrue(need.SiteCode == 4);
		Assert.IsTrue(need.TimeNeeded == new DateTime(2019, 10, 7, 8, 0, 0));
		Assert.IsTrue(need.Amount == 4);
	}

	[TestMethod]
	public async Task RemoveNeedFromMany()
	{
		var user = GetTestUser(EUserAccessLevel.RSO);
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
		var user = GetTestUser(EUserAccessLevel.RSO);
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
		var user = GetTestUser(EUserAccessLevel.RSO);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("WHOIS", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage(user.CoordinatorRoles.Single().WorkingGroup.ShortCode, user.TelegramUser);
		Assert.IsTrue(msg.Content.Contains(user.PhoneNumber));
	}

	[TestMethod]
	public async Task CheckWhoIs_Shortcode()
	{
		var user = GetTestUser(EUserAccessLevel.RSO);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("WHOIS " + user.CoordinatorRoles.Single().WorkingGroup.ShortCode, user.TelegramUser);
		Assert.IsTrue(msg.Content.Contains(user.PhoneNumber));
	}
}