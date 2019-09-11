using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace XR.Dodo
{
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
		public bool IsCoordinator { get { return CoordinatorRoles.Count > 0; } }
		public User()
		{
			UUID = Guid.NewGuid().ToString();
		}
	}
}
