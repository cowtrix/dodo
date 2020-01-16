using Common;
using Common.Extensions;
using REST.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using REST.Serializers;
using System;
using System.Collections.Generic;

namespace REST
{
	public interface IRESTResource : IVerifiable
	{
		Guid GUID { get; }
		string ResourceURL { get; }
		void OnDestroy();
		void AppendAuxilaryData(Dictionary<string, object> view, EPermissionLevel permissionLevel, object requester, Passphrase passphrase);
	}

	public interface IRESTResourceSchema
	{ }

	/// <summary>
	/// A resource is a component that can be interacted with through REST API calls
	/// It has a location on the server (given by ResourceURL) that MUST be unique
	/// It also has a UUID, which can be an alternate accessor using resources/uuid
	/// </summary>
	[BsonIgnoreExtraElements(Inherited = true)]
	public abstract class Resource : IRESTResource
	{
		/// <summary>
		/// The GUID of the resource is unique. You can get a resource
		/// from it's guid by sending a GET request to /resources/{GUID}
		/// which will give you its ResourceURL
		/// </summary>
		[NoPatch]
		[View(EPermissionLevel.USER)]
		[JsonProperty]
		public Guid GUID { get; private set; }

		/// <summary>
		/// The URI of this resource
		/// </summary>
		[View(EPermissionLevel.USER)]
		[ResourceURL]
		public abstract string ResourceURL { get; }

		[View(EPermissionLevel.PUBLIC)]
		[JsonProperty]
		[UserFriendlyName]
		public string Name { get; set; }

		public Resource() { }

		public Resource(string name)
		{
			Name = name;
			GUID = Guid.NewGuid();
		}

		public override bool Equals(object obj)
		{
			var resource = obj as Resource;
			return resource != null &&
				   GUID.Equals(resource.GUID) &&
				   ResourceURL == resource.ResourceURL;
		}

		public override int GetHashCode()
		{
			var hashCode = 1286416240;
			hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(GUID);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ResourceURL);
			return hashCode;
		}

		/// <summary>
		/// This is where resources should clean up after themselves, e.g. removing references to themselves
		/// from other resources and systems.
		/// </summary>
		public virtual void OnDestroy() { }

		public virtual void AppendAuxilaryData(Dictionary<string, object> view, EPermissionLevel permissionLevel,
			object requester, Passphrase passphrase )
		{
			view.Add("PERMISSION", permissionLevel.GetName());
		}

		public bool CanVerify()
		{
			return false;
		}
	}
}
