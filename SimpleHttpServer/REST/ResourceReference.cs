﻿using Common;
using Common.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SimpleHttpServer.REST
{

	public interface IResourceReference : IVerifiable
	{
		Guid Guid { get; }
	}

	public struct ResourceReference<T> : IResourceReference where T : class, IRESTResource
	{
		[View(EPermissionLevel.ADMIN)]
		[JsonProperty]
		public Guid Guid { get; private set; }
		[JsonIgnore]
		public T Value { get { return ResourceUtility.GetResourceByGuid<T>(Guid); } }
		[JsonIgnore]
		public bool HasValue { get { return Value != null; } }
		public ResourceReference(T resource)
		{
			Guid = resource != null ? resource.GUID : default;
		}
		public ResourceReference(Guid guid)
		{
			Guid = guid;
		}
		public static implicit operator T(ResourceReference<T> d) => d.Value;
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

		public void CheckValue()
		{
			if(Value == null)
			{
				throw new Exception("Resource not found with GUID " + Guid);
			}
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
