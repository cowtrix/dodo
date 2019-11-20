using Common;
using System;
using System.Collections.Generic;

namespace SimpleHttpServer.REST
{
	public interface IRESTResource
	{
		Guid GUID { get; }
		string ResourceURL { get; }
		void OnDestroy();
	}

	public interface IRESTResourceSchema
	{ }

	/// <summary>
	/// A resource is a component that can be interacted with through REST API calls
	/// It has a location on the server (given by ResourceURL) that MUST be unique
	/// It also has a UUID, which can be an alternate accessor using resources/uuid
	/// </summary>
	public abstract class Resource : IRESTResource
	{
		[NoPatch]
		[View(EPermissionLevel.USER)]
		public Guid GUID { get; private set; }
		[View(EPermissionLevel.USER)]
		public abstract string ResourceURL { get; }

		public Resource()
		{
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

		public virtual void OnDestroy() { }
	}
}
