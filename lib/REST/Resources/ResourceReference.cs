using Common;
using Common.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Resources
{

	public interface IResourceReference : IVerifiable
	{
		Guid Guid { get; }
		bool HasValue();
	}

	public struct ResourceReference<T> : IResourceReference where T : class, IRESTResource
	{
		[View(EPermissionLevel.PUBLIC)]
		[JsonProperty]
		[BsonElement]
		public Guid Guid { get; private set; }
		[View(EPermissionLevel.PUBLIC)]
		[JsonProperty]
		[BsonElement]
		public string Slug { get; private set; }
		[View(EPermissionLevel.PUBLIC)]
		[JsonProperty]
		[BsonElement]
		public string Type { get; private set; }

		public T GetValue()
		{
			return ResourceUtility.GetResourceByGuid<T>(Guid);
		}

		public bool HasValue() => Guid != default;

		public ResourceReference(T resource)
		{
			Guid = resource != null ? resource.Guid : default;
			Slug = resource != null ? resource.Slug : default;
			Type = resource?.GetType().Name;
		}

		public ResourceReference(Guid guid, string slug, string type)
		{
			Guid = guid;
			Slug = slug;
			Type = type;
		}

		public static implicit operator T(ResourceReference<T> d) => d.GetValue();
		public static implicit operator ResourceReference<T>(T b) => new ResourceReference<T>(b);

		public override bool Equals(object obj)
		{
			return obj is ResourceReference<T> reference &&
				   Guid.Equals(reference.Guid);
		}

		public override int GetHashCode()
		{
			return -737073652 + EqualityComparer<Guid>.Default.GetHashCode(Guid);
		}

		public static bool operator ==(ResourceReference<T> left, ResourceReference<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ResourceReference<T> left, ResourceReference<T> right)
		{
			return !(left == right);
		}

		public bool CanVerify()
		{
			return !ResourceLock.IsLocked(Guid);
		}

		public bool VerifyExplicit(out string error)
		{
			error = null;
			return true;
		}
	}
}
