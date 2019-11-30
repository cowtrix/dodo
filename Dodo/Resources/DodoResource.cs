﻿using Common.Security;
using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Collections.Concurrent;

namespace Dodo
{
	public interface IDodoResource : IRESTResource
	{
		ResourceReference<User> Creator { get; }
		bool IsAuthorised(User requestOwner, Passphrase passphrase, HttpRequest request, out EPermissionLevel permissionLevel);
	}

	public abstract class DodoResource : Resource, IDodoResource
	{
		public DodoResource() : base() { }
		public DodoResource(User creator, string name) : base(name)
		{
			Creator = new ResourceReference<User>(creator);
		}
		public ResourceReference<User> Creator { get; private set; }
		public abstract bool IsAuthorised(User requestOwner, Passphrase passphrase, HttpRequest request, out EPermissionLevel permissionLevel);
	}
}
