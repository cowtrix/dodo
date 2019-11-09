﻿using Common;
using Dodo.Rebellions;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System;

namespace RESTTests
{
	[TestClass]
	public class RebellionTests : RESTTestBase<Rebellion>
	{
		public override string CreationURL => "newrebellion";
		public override object GetCreationSchema()
		{
			return new { RebellionName = "Test Rebellion", Location = new GeoLocation(45, 97) };
		}

		[TestInitialize]
		public void Setup()
		{
			RequestJSON("register", Method.POST, new { Username = CurrentLogin, Password = CurrentPassword, Email = "" });
		}

		[TestMethod]
		public void CannotCreateNewRebellionIfNotLoggedIn()
		{
			AssertX.Throws<Exception>(() =>
			{
				RequestJSON(CreationURL, Method.POST, GetCreationSchema(), "", "");
			}, e => e.Message.Contains("You need to login"));
		}

		public override object GetPatchSchema()
		{
			return new { Location = new GeoLocation(62, 41) };
		}
	}
}