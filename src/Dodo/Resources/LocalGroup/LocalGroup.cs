using Common;
using Dodo.Users;
using REST;
using System;
using Dodo.Roles;
using Common.Extensions;
using REST.Security;
using REST.Serializers;

namespace Dodo.LocalGroups
{
	[Name("Local Group")]
	public class LocalGroup : GroupResource
	{
		public const string ROOT = "localgroups";

		public LocalGroup(LocalGroupSchema schema) : base(schema)
		{
			Location = schema.Location;
		}

		public override string ResourceURL => $"{ROOT}/{Name.StripForURL()}";

		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; private set; }

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
