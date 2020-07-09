using Resources.Security;
using Resources;
using Newtonsoft.Json;
using Common.Extensions;
using Common;
using System;
using System.Collections.Generic;
using Resources.Location;

namespace Dodo
{
	/// <summary>
	/// An owned resource has a "parent" resource that it sits under
	/// E.g. Working Groups, Roles, Sites
	/// </summary>
	public interface IOwnedResource : IDodoResource
	{
		ResourceReference<GroupResource> Parent { get; }
	}

	public interface IDodoResource : IRESTResource
	{
		bool IsCreator(AccessContext context);
	}

	public class NotNulResourceAttribute : CustomValidator
	{
		public override bool Verify(object value, out string validationError)
		{
			if (value is IResourceReference rscRef && rscRef.HasValue())
			{
				validationError = null;
				return true;
			}
			validationError = $"Resource reference as null for {value}";
			return false;
		}
	}

	public abstract class DodoResource : Resource, IDodoResource
	{
		private const string METADATA_PUBLISHED = "published";
		public const string METADATA_NOTIFICATIONS_KEY = "notifications";
		public DodoResource() : base() { }

		public DodoResource(AccessContext creator, ResourceSchemaBase schema) : base(schema)
		{
			if (creator.User != null)
			{
				Creator = SecurityExtensions.GenerateID(creator.User, creator.Passphrase);
			}
		}

		public string Creator { get; private set; }

		public bool IsCreator(AccessContext context)
		{
			if(context.User == null)
			{
				return false;
			}
			return Creator == SecurityExtensions.GenerateID(context.User, context.Passphrase);
		}

		public virtual void OnDestroy()
		{
			if (this is IOwnedResource owned && owned.Parent.HasValue())
			{
				// Remove listing from parent resource if needed				
				using var rscLock = new ResourceLock(owned.Parent.Guid);
				{
					var parent = rscLock.Value as GroupResource;
					if(!parent.RemoveChild(owned))
					{
						throw new Exception($"Unexpectedly failed to remove child object {Guid} from parent resource");
					}
					ResourceUtility.GetManager(parent.GetType()).Update(parent, rscLock);
				}
			}
		}
	}
}
