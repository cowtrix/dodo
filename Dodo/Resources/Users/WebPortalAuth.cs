using Common;
using SimpleHttpServer.REST;
using System;

namespace Dodo.Users
{
	public struct WebPortalAuth
	{
		public WebPortalAuth(string userName, string password) : this()
		{
			Username = userName;
			PasswordHash = SHA256Utility.SHA256(password);
		}

		/// <summary>
		/// The user's username
		/// </summary>
		[View(EViewVisibility.PUBLIC)]
		public string Username { get; private set; }

		/// <summary>
		/// Am MD5 hash of the user's password
		/// </summary>
		public string PasswordHash { get; private set; }

		public bool Challenge(string password)
		{
			return SHA256Utility.SHA256(password) == PasswordHash;
		}

		public void Validate()
		{
			if (string.IsNullOrEmpty(Username))
			{
				throw new Exception("Username cannot be null");
			}
			if (string.IsNullOrEmpty(PasswordHash))
			{
				throw new Exception("PasswordHash cannot be null");
			}
		}
	}
}
