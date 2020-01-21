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
			newResource.Verify();
			ResourceUtility.Register(newResource);
			return newResource;
		}

		protected virtual TResult CreateObjectInternal(TSchema schema)
		{
			return (TResult)Activator.CreateInstance(typeof(TResult), schema);
		}

		protected virtual bool ValidateSchema(TSchema schema, out string error)
		{
			error = null;
			return true;
		}

		public TResult CreateObject(ResourceSchemaBase schema)
		{
			if(!(schema is TSchema))
			{
				return default;
			}
			return CreateObject((TSchema)schema);
		}

		public Type SchemaType => typeof(TSchema);
	}
}
