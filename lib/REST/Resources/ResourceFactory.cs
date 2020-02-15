using Common;
using Common.Extensions;
using System;

namespace Resources
{
	public interface IResourceFactory<T> : IResourceFactory
	{
		Type SchemaType { get; }
		T CreateObject(object context, ResourceSchemaBase schema);
	}

	public interface IResourceFactory { }

	public abstract class ResourceFactory<TResult, TSchema, TContext> 
		: IResourceFactory<TResult>
		where TResult : class, IRESTResource
		where TSchema : ResourceSchemaBase
	{
		public TResult CreateObject(TContext context, TSchema schema)
		{
			if(!ValidateSchema(context, schema, out var error))
			{
				throw new Exception(error);
			}
			var newResource = CreateObjectInternal(context, schema);
			if(!newResource.Verify(out error))
			{
				throw new Exception(error);
			}
			ResourceUtility.Register(newResource);
			Logger.Debug($"Created new resource {newResource.GetType().Name}: {newResource.GUID}");
			return newResource;
		}

		protected virtual TResult CreateObjectInternal(TContext context, TSchema schema)
		{
			return (TResult)Activator.CreateInstance(typeof(TResult), context, schema);
		}

		protected virtual bool ValidateSchema(TContext context, ResourceSchemaBase schema, out string error)
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

		public TResult CreateObject(object context, ResourceSchemaBase schema)
		{
			return CreateObject((TContext)context, (TSchema)schema);
		}

		public Type SchemaType => typeof(TSchema);
	}
}
