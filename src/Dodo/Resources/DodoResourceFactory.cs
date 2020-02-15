using Dodo.Users;
using Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dodo.Resources
{
	public abstract class DodoResourceFactory<TResult, TSchema>	: ResourceFactory<TResult, TSchema, AccessContext>
		where TResult : class, IRESTResource
		where TSchema : DodoResourceSchemaBase
	{
		protected override bool ValidateSchema(AccessContext context, ResourceSchemaBase schemaBase, out string error)
		{
			if(!base.ValidateSchema(context, schemaBase, out error))
			{
				return false;
			}
			if(!context.Challenge())
			{
				error = "Bad authorisation";
				return false;
			}
			return true;
		}
	}
}
