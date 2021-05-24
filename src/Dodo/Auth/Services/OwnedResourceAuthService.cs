using Resources;

namespace Dodo
{
	public class OwnedResourceAuthService<T, TSchema> : AuthorizationService<T, TSchema>
		where T : class, IOwnedResource, IDodoResource
		where TSchema : OwnedResourceSchemaBase
	{
		public OwnedResourceAuthService() : base()
		{
		}

		protected override IRequestResult CanCreate(AccessContext context, TSchema target)
		{
			var parent = target.GetParent() as IAdministratedResource;
			if (parent == null)
			{
				return ResourceRequestError.BadRequest();
			}
			if (!parent.IsAdmin(context.User, context, out var p))
			{
				return ResourceRequestError.UnauthorizedRequest();
			}
			return new ResourceCreationRequest(context, target);
		}
	}
}
