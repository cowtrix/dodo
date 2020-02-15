﻿using Resources;
using System;
using Common.Extensions;
using Common;
using Resources.Serializers;
using Dodo.Resources;

namespace Dodo.LocalGroups
{
	public class LocalGroupSerializer : ResourceReferenceSerializer<LocalGroup> { }

	public class LocalGroupSchema : GroupResourceSchemaBase
	{
		public GeoLocation Location { get; private set; }

		public LocalGroupSchema()
		{
		}

		public LocalGroupSchema(string name, string description, GeoLocation location) 
			: base(name, description, default)
		{
			Location = location;
		}
	}

	public class LocalGroupFactory : DodoResourceFactory<LocalGroup, LocalGroupSchema>
	{
	}
}
