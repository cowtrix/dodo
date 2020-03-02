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
		Guid GUID { get; }
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
	}

	/// <summary>
	/// A resource is a component that can be interacted with through REST API calls
	/// It has a location on the server (given by ResourceURL) that MUST be unique
	/// It also has a UUID, which can be an alternate accessor using resources/uuid
	/// </summary>
	[BsonIgnoreExtraElements(Inherited = true)]
	public abstract class Resource : IRESTResource
	{
		public const string METADATA = "METADATA";
		public const string METADATA_PERMISSION = "PERMISSION";

		/// <summary>
		/// The GUID of the resource is unique. You can get a resource
		/// from it's guid by sending a GET request to /resources/{GUID}
		/// which will give you its ResourceURL
		/// </summary>
		[NoPatch]
		[View(EPermissionLevel.PUBLIC)]
		[JsonProperty]
		public Guid GUID { get; private set; }

		[View(EPermissionLevel.PUBLIC)]
		[JsonProperty]
		[UserFriendlyName]
		public string Name { get; set; }

		public Resource(ResourceSchemaBase schema)
		{
			Name = schema?.Name;
			GUID = Guid.NewGuid();
		}

		public override bool Equals(object obj)
		{
			var resource = obj as Resource;
			return resource != null &&
				   GUID.Equals(resource.GUID);
		}

		public override int GetHashCode()
		{
			var hashCode = 1286416240;
			hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(GUID);
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
		}

		public bool CanVerify()
		{
			return false;
		}
	}
}
