using Common;
using Dodo.Users;
using Dodo.Rebellions;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Collections.Concurrent;
using System;
using Dodo.Roles;

namespace Dodo.LocalGroups
{
	public class LocalGroup : GroupResource
	{
		public const string ROOT = "localgroups";
		public LocalGroup(User owner, string passphrase, LocalGroupRESTHandler.CreationSchema schema) : base(owner, passphrase, null)
		{
			Name = schema.Name;
			Location = schema.Location;
		}

		[View(EPermissionLevel.USER)]
		[NoPatch]
		public string Name { get; private set; }

		public override string ResourceURL => $"{ROOT}/{Name.StripForURL()}";

		[View(EPermissionLevel.USER)]
		public GeoLocation Location { get; private set; }

		public override bool IsAuthorised(User requestOwner, string passphrase, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			if (IsAdmin(requestOwner, passphrase))
			{
				permissionLevel = EPermissionLevel.ADMIN;
				return true;
			}
			if(requestOwner == Creator.Value)
			{
				permissionLevel = EPermissionLevel.OWNER;
				return true;
			}
			permissionLevel = EPermissionLevel.USER;
			return true;
		}

		public override bool CanContain(Type type)
		{
			if (type == typeof(Role))
			{
				return true;
			}
			return false;
		}
	}
}
