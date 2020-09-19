using Common;
using Common.Extensions;
using Dodo.Users;
using Dodo.Users.Tokens;
using Resources;
using System;

namespace Dodo.DodoResources
{
	public abstract class DodoResourceFactory<TResult, TSchema> : ResourceFactory<TResult, TSchema, ResourceCreationRequest>
		where TResult : class, IDodoResource
		where TSchema : ResourceSchemaBase
	{
		private static object m_factoryLock = new object();

		protected override bool ValidateSchema(ResourceCreationRequest request, out string error)
		{
			if (!base.ValidateSchema(request, out error))
			{
				return false;
			}
			if (request.Schema == null)
			{
				error = "Schema cannot be null";
				return false;
			}
			if (!request.AccessContext.Challenge())
			{
				error = "Bad authorisation";
				return false;
			}
			return true;
		}

		protected override TResult CreateObjectInternal(ResourceCreationRequest request)
		{
			lock (m_factoryLock)
			{
				if (request.Token != default)
				{
					using (var rscLock = new ResourceLock(request.AccessContext.User))
					{
						var creator = rscLock.Value as User;
						creator.TokenCollection.GetToken<ResourceCreationToken>(request.AccessContext, request.Token);
						ResourceUtility.GetManager<User>().Update(creator, rscLock);
					}
				}
				var newResource = (TResult)Activator.CreateInstance(typeof(TResult), request.AccessContext, request.Schema);
				if (newResource == null)
				{
					throw new Exception($"Failed to create resource of type {typeof(TResult)} from context {request?.GetType()}");
				}
				if (!newResource.Verify(out var error))
				{
					throw new Exception(error);
				}
				ResourceUtility.Register(newResource);
				Logger.Debug($"Created new resource {newResource.GetType().Name}: {newResource.Guid}");
				if (newResource is IOwnedResource owned && owned.Parent.HasValue())
				{
					// Add listing to parent resource if needed
					using var rscLock = new ResourceLock(owned.Parent.Guid);
					{
						var parent = rscLock.Value as AdministratedGroupResource;
						parent.AddChild(owned);
						ResourceUtility.GetManager(parent.GetType()).Update(parent, rscLock);
					}
				}
				return newResource;
			}
		}
	}
}
