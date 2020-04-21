using Common;
using Common.Extensions;
using Resources.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources.Serializers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Resources
{
	public interface IRESTResource : IVerifiable
	{
		Guid Guid { get; }
		uint Revision { get; set; }
		string Name { get; }
		void OnDestroy();
		void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel, object requester, Passphrase passphrase);
	}

	public abstract class ResourceSchemaBase : IVerifiable
	{
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
	}

	/// <summary>
	/// A resource is a component that can be interacted with through REST API calls
	/// </summary>
	[BsonIgnoreExtraElements(Inherited = true)]
	public abstract class Resource : IRESTResource
	{
		public const string TYPE = "type";
		public const string METADATA = "metadata";
		public const string METADATA_PERMISSION = "permission";

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		[JsonProperty]
		public Guid Guid { get; private set; }

		[View(EPermissionLevel.PUBLIC)]
		[JsonProperty]
		[UserFriendlyName]
		public string Name { get; set; }

		/// <summary>
		/// This should only ever be incremented on ResourceManager.Update()
		/// </summary>
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		[JsonProperty]
		public uint Revision { get; set; }

		public Resource(ResourceSchemaBase schema)
		{
			Name = schema?.Name;
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
			view.Add(METADATA_PERMISSION, permissionLevel.GetName());
			view.Add(TYPE, GetType().Name);
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
	}
}
