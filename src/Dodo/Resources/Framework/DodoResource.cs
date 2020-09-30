using Resources.Security;
using Resources;
using System;

namespace Dodo
{
	public abstract class DodoResource : Resource, IDodoResource
	{
		public DodoResource() : base() { }

		public DodoResource(AccessContext creator, ResourceSchemaBase schema) : base(schema)
		{
			if (creator.User != null)
			{
				Creator = SecurityExtensions.GenerateID(creator.User, creator.Passphrase, Guid.ToString());
			}
			if(this is IOwnedResource owned && schema is OwnedResourceSchemaBase ownedSchema)
			{
				var group = ownedSchema.GetParent();
				if (group == null)
				{
					throw new Exception($"Invalid parent group ID in construction: {ownedSchema.ParentID}");
				}
				owned.Parent = group.CreateRef<IRESTResource>();
			}
		}

		public string Creator { get; private set; }

		public bool IsCreator(AccessContext context)
		{
			if (context.User == null)
			{
				return false;
			}
			return Creator == SecurityExtensions.GenerateID(context.User, context.Passphrase, Guid.ToString());
		}

		public override void OnDestroy()
		{
			if (this is IOwnedResource owned && owned.Parent.HasValue())
			{
				// Remove listing from parent resource if needed				
				using var rscLock = new ResourceLock(owned.Parent.Guid);
				{
					var parent = rscLock.Value as AdministratedGroupResource;
					if (!parent.RemoveChild(owned))
					{
						throw new Exception($"Unexpectedly failed to remove child object {Guid} from parent resource");
					}
					ResourceUtility.GetManager(parent.GetType()).Update(parent, rscLock);
				}
			}
		}

		public virtual string GetUrl() => $"{Dodo.DodoApp.NetConfig.FullURI}/{GetType().Name}/{Slug}";
	}
}
