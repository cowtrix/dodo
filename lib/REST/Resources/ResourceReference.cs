using Common;
using Common.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources.Location;
using System;
using System.Collections.Generic;

namespace Resources
{

	public interface IResourceReference : IVerifiable
	{
		Guid Guid { get; }
		string Slug { get; }
		string Type { get; }
		string Name { get; }
		bool HasValue();
	}

	public struct ResourceReference<T> : IResourceReference where T : class, IRESTResource
	{
		[View(EPermissionLevel.PUBLIC)]
		[JsonProperty]
		[BsonElement]
		public string Name { get; private set; }
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
		[JsonProperty]
		[BsonElement]
		public GeoLocation Location { get; set; }
		[JsonProperty]
		[BsonElement]
		public string FullyQualifiedName { get; set; }

		public T GetValue()
		{
			return ResourceUtility.GetResourceByGuid<T>(Guid);
		}

		public Type GetRefType() => System.Type.GetType(FullyQualifiedName);

		public bool HasValue() => Guid != default;

		public ResourceReference(T resource)
		{
			Guid = resource != null ? resource.Guid : default;
			Slug = resource != null ? resource.Slug : default;
			Type = resource?.GetType().Name.ToCamelCase();
			Name = resource?.Name;
			Location = resource is ILocationalResource loc ? loc.Location : null;
			FullyQualifiedName = resource?.GetType().AssemblyQualifiedName;
		}

		public ResourceReference(Guid guid, string slug, Type type, string name, GeoLocation location)
		{
			if(type == null && !string.IsNullOrEmpty(name))
			{
				throw new Exception("Failed to deserialize type");
			}
			Guid = guid;
			Slug = slug;
			Type = type?.Name.ToCamelCase();
			Name = name;
			Location = location;
			FullyQualifiedName = type?.AssemblyQualifiedName;
		}

		public ResourceReference(Guid guid) : this(ResourceUtility.GetResourceByGuid(guid) as T)
		{
		}

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
