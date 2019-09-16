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
		public int TelegramUser;
		public int SiteCode;
		public string UUID;
		public string Email;

		[NonSerialized]
		public HashSet<WorkingGroup> CoordinatorRoles = new HashSet<WorkingGroup>();

		public bool IsVerified()
		{
			return !string.IsNullOrEmpty(PhoneNumber) && TelegramUser != 0;
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
					if(CoordinatorRoles.Any(x => x.ParentGroup == EParentGroup.MovementSupport &&
						x.Name.ToUpperInvariant().Contains("ROTA")))
					{
						return EUserAccessLevel.RotaCoordinator;
					}
					return EUserAccessLevel.Coordinator;
				}
				return EUserAccessLevel.Volunteer;
			}
		}

		public User()
		{
			UUID = Guid.NewGuid().ToString();
		}
	}
}
