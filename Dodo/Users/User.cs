using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XR.Dodo
{
	public enum EUserAccessLevel
	{
		Volunteer,
		Coordinator,
		RotaCoordinator,
		RSO,
	}

	public class User
	{
		public string Name;
		public string PhoneNumber;
		public int TelegramUser = -1;
		public int SiteCode = -1;
		public string UUID;
		public string Email;
		public int Karma;

		public bool GDPR;
		public DateTime StartDate;
		public DateTime EndDate;

		public HashSet<Role> CoordinatorRoles = new HashSet<Role>();
		public HashSet<string> WorkingGroupPreferences = new HashSet<string>();

		public override string ToString()
		{
			return $"{Name ?? PhoneNumber ?? TelegramUser.ToString()} ({AccessLevel})";
		}

		public bool IsVerified()
		{
			return !string.IsNullOrEmpty(PhoneNumber) && TelegramUser >= 0;
		}

		[JsonIgnore]
		public EUserAccessLevel AccessLevel
		{
			get
			{
				if(CoordinatorRoles.Count > 0)
				{
					if(CoordinatorRoles.Any(x => x.SiteCode == 0))
					{
						return EUserAccessLevel.RSO;
					}
					if(IsRotaCoordinator)
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
				return CoordinatorRoles.Any(x => //x.WorkingGroup.ParentGroup == EParentGroup.MovementSupport &&
					  x.WorkingGroup.Name.ToUpperInvariant().Contains("ROTA")); }
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
	}
}
