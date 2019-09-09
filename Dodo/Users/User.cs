using System;

namespace XR.Dodo
{
	public abstract class User
	{
		public string Name;
		public string PhoneNumber;
		public int TelegramUser;
		public int SiteCode;
		public string UUID;

		public bool Verified
		{
			get
			{
				return !string.IsNullOrEmpty(PhoneNumber) && TelegramUser != 0;
			}
		}
		public User()
		{
			UUID = Guid.NewGuid().ToString();
		}
	}
}
