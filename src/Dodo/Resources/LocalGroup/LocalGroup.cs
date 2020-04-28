using Common;
using Dodo.Users;
using Resources;
using System;
using Dodo.Roles;
using Common.Extensions;
using Resources.Security;
using Resources.Serializers;
using Resources.Location;

namespace Dodo.LocalGroups
{
	[Name("Local Group")]
	public class LocalGroup : GroupResource, ILocationalResource
	{
		public const string ROOT = "localgroups";

		public LocalGroup(AccessContext context, LocalGroupSchema schema) : base(context, schema)
		{
			Location = schema.Location;
		}

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
