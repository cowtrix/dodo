using Common;
using Common.Extensions;
using System;

namespace REST
{
	public interface IResourceFactory<T> : IResourceFactory
	{
		Type SchemaType { get; }
		T CreateObject(ResourceSchemaBase schema);
	}

	public interface IResourceFactory { }

	public abstract class ResourceFactory<TResult, TSchema> 
		: IResourceFactory<TResult>
		where TResult : class, IRESTResource
		where TSchema : ResourceSchemaBase
	{
		public TResult CreateObject(TSchema schema)
		{
			if(!ValidateSchema(schema, out var error))
			{
				throw new Exception(error);
			}
			var newResource = CreateObjectInternal(schema);
			if(!newResource.Verify(out error))
			{
				throw new Exception(error);
			}
			ResourceUtility.Register(newResource);
			Logger.Debug($"Created new resource {newResource.GetType().Name}: {newResource.GUID}");
			return newResource;
		}

		protected virtual TResult CreateObjectInternal(TSchema schema)
		{
			return (TResult)Activator.CreateInstance(typeof(TResult), schema);
		}

		protected virtual bool ValidateSchema(ResourceSchemaBase schema, out string error)
		{
			if (schema == null)
			{
				throw new NullReferenceException("Schema cannot be null");
			}
			if (!(schema is TSchema))
			{
				throw new InvalidCastException($"Incorrect schema type. Expected: {typeof(TSchema).FullName}\t Actual: {schema.GetType().FullName}");
			}
			return schema.Verify(out error);
		}

		public TResult CreateObject(ResourceSchemaBase schema)
		{
			return CreateObject((TSchema)schema);
		}

		public Type SchemaType => typeof(TSchema);
	}
}
