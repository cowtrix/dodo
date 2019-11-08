using System;
using System.Collections.Generic;

namespace SimpleHttpServer.REST
{
	public struct ResourceReference<T> where T : Resource
	{
		public Guid Guid;
		public T Value { get { return ResourceUtility.GetResourceByGuid<T>(Guid); } }
		public ResourceReference(T resource)
		{
			Guid = resource != null ? resource.GUID : default;
		}

		public static implicit operator T(ResourceReference<T> d) => d.Value;
		public static explicit operator ResourceReference<T>(T b) => new ResourceReference<T>(b);

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
	}
}
