using Common;
using Common.Security;
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
			PassPhrase = new EncryptedStore<string>(Guid.NewGuid().ToString(), password);
		}

		/// <summary>
		/// The user's username
		/// </summary>
		[View(EPermissionLevel.USER)]
		public string Username { get; private set; }

		/// <summary>
		/// Am MD5 hash of the user's password
		/// </summary>
		public string PasswordHash { get; private set; }

		public EncryptedStore<string> PassPhrase;

		public bool Challenge(string password, out string passphrase)
		{
			if(SHA256Utility.SHA256(password) != PasswordHash)
			{
				passphrase = null;
				return false;
			}
			passphrase = PassPhrase.GetValue(password);
			return true;
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
