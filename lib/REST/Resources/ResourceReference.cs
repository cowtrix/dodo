using Common;
using Common.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources.Location;
using System;
using System.Collections.Generic;

namespace Resources
{
	/// <summary>
	/// An owned resource has a "parent" resource that it sits under
	/// E.g. Working Groups, Roles, Sites
	/// </summary>
	public interface IOwnedResource : IRESTResource
	{
		ResourceReference<IRESTResource> Parent { get; }
	}

	public interface IDescribedResource : IRESTResource
	{
		public const int SHORT_DESC_LENGTH = 256;
		string PublicDescription { get; }
	}

	public interface IResourceReference : IVerifiable
	{
		Guid Guid { get; }
		string Slug { get; }
		string Type { get; }
		string Name { get; }
		Guid Parent { get; }
		GeoLocation Location { get; }
		string PublicDescription { get; }
		bool HasValue();
		Type GetRefType();
	}

	public struct ResourceReference<T> : IResourceReference where T : class, IRESTResource
	{
		[JsonProperty]
		[BsonElement]
		public string Name { get; private set; }
		[JsonProperty]
		[BsonElement]
		public Guid Guid { get; private set; }
		[JsonProperty]
		[BsonElement]
		public string Slug { get; private set; }
		[JsonProperty]
		[BsonElement]
		public string Type { get; private set; }
		[JsonProperty]
		[BsonElement]
		public GeoLocation Location { get; set; }
		[JsonProperty]
		[BsonElement]
		public string FullyQualifiedName { get; set; }
		[JsonProperty]
		[BsonElement]
		public Guid Parent { get; set; }
		[JsonProperty]
		[BsonElement]
		public string PublicDescription { get; set; }

		public T GetValue()
		{
			return ResourceUtility.GetResourceByGuid<T>(Guid);
		}

		public T2 GetValue<T2>() where T2: class, T
		{
			return GetValue() as T2;
		}

		public Type GetRefType() => string.IsNullOrEmpty(FullyQualifiedName) ? null : System.Type.GetType(FullyQualifiedName);

		public bool HasValue() => Guid != default;

		public ResourceReference(T resource)
		{
			Guid = resource != null ? resource.Guid : default;
			Slug = resource != null ? resource.Slug : default;
			Type = resource?.GetType().Name.ToCamelCase();
			Name = resource?.Name;
			Location = resource is ILocationalResource loc ? loc.Location : null;
			FullyQualifiedName = resource?.GetType().AssemblyQualifiedName;
			if(resource is IOwnedResource owned)
			{
				Parent = owned.Parent.Guid;
			}
			else
			{
				Parent = default;
			}
			if(resource is IDescribedResource desc)
			{
				PublicDescription = StringExtensions.StripMDLinks(desc.PublicDescription?.Substring(0, Math.Min(desc.PublicDescription.Length, IDescribedResource.SHORT_DESC_LENGTH)));
			}
			else
			{
				PublicDescription = default;
			}
		}

		public ResourceReference(Guid guid, string slug, Type type, string name, GeoLocation location, Guid parent, string desc)
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
			Parent = parent;
			PublicDescription = desc;
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
