using System.Threading.Tasks; 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DodoTest;
using XR.Dodo;
using System;

[TestClass]
public class InfoTaskTests : TestBase
{
	[TestMethod]
	public async Task ChangeEmail()
	{
		var newEmail = "test@changed.com";
		var user = GetTestUser(EUserAccessLevel.Volunteer);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("INFO", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("email", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage(newEmail, user.TelegramUser);
		Assert.IsTrue(user.Email == newEmail);
	}

	[TestMethod]
	public async Task CannotSetInvalidEmail()
	{
		var newEmail = "this is invalid";
		var user = GetTestUser(EUserAccessLevel.Volunteer);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("INFO", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("email", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage(newEmail, user.TelegramUser);
		Assert.IsFalse(user.Email == newEmail);
	}

	[TestMethod]
	public async Task CanChangeStartDate()
	{
		var newStart = new DateTime(2019, 10, 8);
		var user = GetTestUser(EUserAccessLevel.Volunteer);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("Info", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("Arrival", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage(newStart.ToString("dd/MM"), user.TelegramUser);
		Assert.IsTrue(user.StartDate == newStart);
	}

	[TestMethod]
	public async Task CannotSetStartDateBeforeRebellionStarts()
	{
		var newStart = new DateTime(2019, 10, 6);
		var user = GetTestUser(EUserAccessLevel.Volunteer);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("Info", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("Arrival", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage(newStart.ToString("dd/MM"), user.TelegramUser);
		Assert.IsFalse(user.StartDate == newStart);
	}

	[TestMethod]
	public async Task CanChangeEndDate()
	{
		var newEnd = new DateTime(2019, 10, 10);
		var user = GetTestUser(EUserAccessLevel.Volunteer);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("Info", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("departure", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage(newEnd.ToString("dd/MM"), user.TelegramUser);
		Assert.IsTrue(user.EndDate == newEnd);
	}

	[TestMethod]
	public async Task CannotSetEndDateBeforeStart()
	{
		var newEnd = new DateTime(2019, 10, 8);
		var user = GetTestUser(EUserAccessLevel.Volunteer);
		user.StartDate = new DateTime(2019, 10, 9);
		var session = DodoServer.SessionManager.GetOrCreateSession(user);
		var msg = DodoServer.TelegramGateway.FakeMessage("Info", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage("departure", user.TelegramUser);
		msg = DodoServer.TelegramGateway.FakeMessage(newEnd.ToString("dd/MM"), user.TelegramUser);
		Assert.IsFalse(user.EndDate == newEnd);
	}
}
