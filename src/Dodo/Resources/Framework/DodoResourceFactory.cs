using Common;
using Common.Extensions;
using Dodo.Users;
using Dodo.Users.Tokens;
using Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dodo.Resources
{
	public abstract class DodoResourceFactory<TResult, TSchema> : ResourceFactory<TResult, TSchema, AccessContext>
		where TResult : class, IRESTResource
		where TSchema : ResourceSchemaBase
	{
		protected override bool ValidateSchema(AccessContext context, ResourceSchemaBase schemaBase, out string error)
		{
			if (!base.ValidateSchema(context, schemaBase, out error))
			{
				return false;
			}
			if (!context.Challenge())
			{
				error = "Bad authorisation";
				return false;
			}
			return true;
		}

		public TResult CreateObject(ResourceCreationRequest request)
		{
			if (!ValidateSchema(request.AccessContext, request.Schema, out var error))
			{
				throw new Exception(error);
			}
			var newResource = CreateObjectInternal(request);
			if (newResource == null)
			{
				throw new Exception($"Failed to create resource of type {typeof(TResult)} from schema {request?.Schema?.GetType()}");
			}
			if (!newResource.Verify(out error))
			{
				throw new Exception(error);
			}
			ResourceUtility.Register(newResource);
			Logger.Debug($"Created new resource {newResource.GetType().Name}: {newResource.Guid}");
			return newResource;
		}

		protected TResult CreateObjectInternal(ResourceCreationRequest request)
		{
			if (request.Token != null)
			{
				using (var rscLock = new ResourceLock(request.AccessContext.User))
				{
					var creator = rscLock.Value as User;
					creator.TokenCollection.GetToken<ResourceCreationToken>(request.AccessContext, request.Token);
					ResourceUtility.GetManager<User>().Update(creator, rscLock);
				}
			}

			var rsc = base.CreateObjectInternal(request.AccessContext, request.Schema as TSchema);
			if (rsc is IOwnedResource owned && owned.Parent.HasValue())
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
