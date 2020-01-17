using Common.Extensions;
using System;

namespace REST
{
	public abstract class ResourceFactory<TResult, TSchema> 
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

		public Type SchemaType => typeof(TSchema);
	}
}
