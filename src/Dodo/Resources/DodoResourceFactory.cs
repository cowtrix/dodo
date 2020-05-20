using Dodo.Users;
using Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dodo.Resources
{
	public abstract class DodoResourceFactory<TResult, TSchema>	: ResourceFactory<TResult, TSchema, AccessContext>
		where TResult : class, IRESTResource
		where TSchema : ResourceSchemaBase
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

		protected override TResult CreateObjectInternal(AccessContext context, TSchema schema)
		{
			var rsc = base.CreateObjectInternal(context, schema);
			if(rsc is IOwnedResource owned && owned.Parent.HasValue())
			{
				// Add listing to parent resource if needed				
				using var rscLock = new ResourceLock(owned.Parent.Guid);
				{
					var parent = rscLock.Value as GroupResource;
					parent.AddChild(owned);
					ResourceUtility.GetManager(parent.GetType()).Update(parent, rscLock);
				}
			}
			return rsc;
		}
	}
}
