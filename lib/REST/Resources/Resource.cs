using Common;
using Common.Extensions;
using Resources.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources.Serializers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Resources
{
	public class IRESTResourceReferenceSerializer : ResourceReferenceSerializer<IRESTResource> { }

	public interface IRESTResource : IVerifiable, IViewMetadataProvider
	{
		Guid Guid { get; }
		uint Revision { get; set; }
		string Name { get; }
		string Slug { get; }
		void OnDestroy();
	}

	public interface IViewMetadataProvider
	{
		void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel, object requester, Passphrase passphrase);
	}

	public abstract class ResourceSchemaBase : IVerifiable
	{
		[View(EPermissionLevel.PUBLIC, customDrawer:"slugPreview", inputHint:"Must be between 3-64 characters")]
		[UserFriendlyName]
		public string Name { get; set; }

		public ResourceSchemaBase(string name)
		{
			Name = name;
		}

		public ResourceSchemaBase() { }

		public bool CanVerify()
		{
			return true;
		}

		public virtual bool VerifyExplicit(out string error)
		{
			error = null;
			return true;
		}

		public abstract Type GetResourceType();

		public virtual void OnView(object requester, Passphrase passphrase)
		{
		}
	}

	/// <summary>
	/// A resource is a component that can be interacted with through REST API calls
	/// </summary>
	[BsonIgnoreExtraElements(Inherited = true)]
	public abstract class Resource : IRESTResource
	{
		[BsonIgnore]
		[JsonIgnore]
		protected IResourceManager ResourceManager => ResourceUtility.GetManager(this.GetType());

		public const string METADATA_TYPE = "type";
		public const string METADATA = "metadata";
		public const string METADATA_PERMISSION = "permission";

		[JsonProperty]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM, customDrawer: "null")]
		public Guid Guid { get; private set; }

		[View(EPermissionLevel.PUBLIC, priority: 0)]
		[JsonProperty]
		[UserFriendlyName]
		public string Name { get; set; }

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN, priority:1, customDrawer:"null")]
		[JsonProperty]
		[Slug]
		public string Slug { get; set; }

		/// <summary>
		/// This should only ever be incremented on ResourceManager.Update()
		/// </summary>
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM, customDrawer: "null")]
		[JsonProperty]
		public uint Revision { get; set; }

		public Resource() { }

		public Resource(ResourceSchemaBase schema)
		{
			if(schema == null)
			{
				throw new ArgumentNullException(nameof(schema));
			}
			Name = schema.Name;
			Slug = ValidationExtensions.StripStringForSlug(Name);
			while(ResourceManager.Get(r => r.Slug == Slug, force:true).Any())
			{
				Slug += Common.Security.KeyGenerator.GetUniqueKey(2).ToLowerInvariant();
			}
			Guid = Guid.NewGuid();
		}

		public override bool Equals(object obj)
		{
			var resource = obj as Resource;
			return resource != null &&
				   Guid.Equals(resource.Guid);
		}

		public override int GetHashCode()
		{
			var hashCode = 1286416240;
			hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(Guid);
			return hashCode;
		}

		/// <summary>
		/// This is where resources should clean up after themselves, e.g. removing references to themselves
		/// from other resources and systems.
		/// </summary>
		public virtual void OnDestroy() { }

		public virtual void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel,
			object requester, Passphrase passphrase )
		{
			view.Add(METADATA_PERMISSION, permissionLevel.GetName().ToLowerInvariant());
			view.Add(METADATA_TYPE, GetType().Name.ToCamelCase());
		}

		public bool CanVerify()
		{
			return false;
		}

		public virtual bool VerifyExplicit(out string error)
		{
			error = null;
			return true;
		}

		public override string ToString() => $"[{Guid.ToString().Substring(0, 6)}] {Name}";
	}
}
