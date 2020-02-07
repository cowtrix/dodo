using Dodo.Users;
using REST;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dodo.Resources
{
	public abstract class DodoResourceFactory<TResult, TSchema>
		: ResourceFactory<TResult, TSchema>
		where TResult : class, IRESTResource
		where TSchema : DodoResourceSchemaBase
	{
		protected override bool ValidateSchema(ResourceSchemaBase schemaBase, out string error)
		{
			if(!base.ValidateSchema(schemaBase, out error))
			{
				return false;
			}
			var schema = (TSchema)schemaBase;
			if(schema == null || !schema.Context.Challenge())
			{
				error = "Bad Context";
				return false;
			}
			return true;
		}
	}
}
