using Common;
using Common.Extensions;
using System;

namespace Resources
{
	public interface IResourceFactory<T> : IResourceFactory
	{
		Type SchemaType { get; }
		T CreateTypedObject(object context, ResourceSchemaBase schema);
	}

	public interface IResourceFactory 
	{
		IRESTResource CreateObject(object context, ResourceSchemaBase schema);
	}

	public abstract class ResourceFactory<TResult, TSchema, TContext> 
		: IResourceFactory<TResult>
		where TResult : class, IRESTResource
		where TSchema : ResourceSchemaBase
	{
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

		public IRESTResource CreateObject(object context, ResourceSchemaBase schema)
		{
			return CreateObject((TContext)context, (TSchema)schema);
		}

		public TResult CreateTypedObject(object context, ResourceSchemaBase schema)
		{
			var obj = CreateObject(context, schema);
			if(!(obj is TResult typedObj))
			{
				throw new Exception($"Could not create object: Generated incorrect type {obj.GetType()}");
			}
			return typedObj;
		}

		public Type SchemaType => typeof(TSchema);
	}
}
