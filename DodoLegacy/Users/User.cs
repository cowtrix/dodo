﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dodo.Dodo
{
	public enum EUserAccessLevel
	{
		Volunteer,
		Coordinator,
		RotaCoordinator,
		RSO,
		Admin,
	}

	public delegate void OnMessageReceivedEvent(ServerMessage message, UserSession session);

	public class User
	{
		public enum ESpamSetting
		{
			DEFAULT,
			MORE,
			LESS,
		}

		public string Name;
		[Phone]
		public string PhoneNumber;
		public int TelegramUser = -1;
		public int SiteCode = -1;
		public string UUID;
		public string Email;
		public int Karma;
		public bool Active;
		public bool GDPR;
		public DateTime StartDate;
		public DateTime EndDate = DateTime.MaxValue;
		public ESpamSetting SpamSetting = ESpamSetting.DEFAULT;
		public int RequestsSent;

		[JsonIgnore]
		public HashSet<Role> CoordinatorRoles = new HashSet<Role>();
		public HashSet<string> WorkingGroupPreferences = new HashSet<string>();

		[JsonIgnore]
		public OnMessageReceivedEvent OnMsgReceived;

		[JsonIgnore]
		public EUserAccessLevel AccessLevel
		{
			get
			{
				if(DodoServer.IsAdmin(this))
				{
					return EUserAccessLevel.Admin;
				}
				if(CoordinatorRoles.Count > 0)
				{
					if(CoordinatorRoles.Any(x => x.SiteCode == 0))
					{
						return EUserAccessLevel.RSO;
					}
					if(IsRotaCoordinator || IsCoordinator)
					{
						return EUserAccessLevel.RotaCoordinator;
					}
					return EUserAccessLevel.Coordinator;
				}
				return EUserAccessLevel.Volunteer;
			}
		}

		[JsonIgnore]
		public bool IsRotaCoordinator
		{
			get {
				return CoordinatorRoles.Any(x => x.WorkingGroup.Name.ToUpperInvariant().Contains("ROTA")); }
		}

		[JsonIgnore]
		public bool IsCoordinator
		{
			get
			{
				return CoordinatorRoles.Any(x => x.Name.ToUpperInvariant().Contains("INTERNAL COORDINATOR") 
					|| x.Name.ToUpperInvariant().Contains("EXTERNAL COORDINATOR")
					|| x.WorkingGroup.Name.ToUpperInvariant().Contains("INTEGRATION"));
			}
		}

		[JsonIgnore]
		public SiteSpreadsheet Site { get { return DodoServer.SiteManager.GetSite(SiteCode); } }

		public User()
		{
			UUID = Guid.NewGuid().ToString();
		}

		public int GetTrustScore()
		{
			int trust = 0;
			if(AccessLevel > EUserAccessLevel.Volunteer)
			{
				// We basically always trust anyone on a spreadsheet
				trust += 5000;
			}
			if(GDPR)
			{
				trust += 10;
			}
			if(IsVerified())
			{
				trust += 100;
			}
			trust += Karma;
			return trust;
		}

		public override string ToString()
		{
			return $"{Name ?? PhoneNumber ?? TelegramUser.ToString()} ({AccessLevel}|{UUID.Substring(0, 8)})";
		}

		public bool IsVerified()
		{
			return !string.IsNullOrEmpty(PhoneNumber) && TelegramUser >= 0;
		}
	}
}