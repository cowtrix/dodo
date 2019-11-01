using Common;
using SimpleHttpServer.REST;
using System;

namespace Dodo.Users
{
	public struct WebPortalAuth
	{
		public WebPortalAuth(string userName, string passwordHash) : this()
		{
			Username = userName;
			PasswordHash = passwordHash;
		}

		/// <summary>
		/// The user's username
		/// </summary>
		[View]
		public string Username { get; private set; }

		/// <summary>
		/// Am MD5 hash of the user's password
		/// </summary>
		public string PasswordHash { get; private set; }

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

	public class User : IRESTResource
	{
		public string ResourceURL { get { return $"u/{WebAuth.Username}"; } }

		/// <summary>
		/// The unique identifier for this user
		/// </summary>
		[NoPatch]
		[View]
		public string UUID { get; private set; }

		[NoPatch]
		[View]
		public WebPortalAuth WebAuth;

		public User(WebPortalAuth auth)
		{
			UUID = System.Guid.NewGuid().ToString();
			WebAuth = auth;
		}

		public bool IsValidToken(string token)
		{
			// TODO
			return true;
		}
	}
}
