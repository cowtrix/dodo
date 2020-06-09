using Common;
using Common.Extensions;
using System;

namespace Resources
{
	public interface ICreationContext : IVerifiable
	{
		ResourceSchemaBase GetSchema();
	}

	public interface IResourceFactory<T> : IResourceFactory
	{
		Type SchemaType { get; }
		T CreateTypedObject(ICreationContext context);
	}

	public interface IResourceFactory 
	{
		IRESTResource CreateObject(ICreationContext context);
	}

	public abstract class ResourceFactory<TResult, TSchema, TContext> 
		: IResourceFactory<TResult>
		where TResult : class, IRESTResource
		where TSchema : ResourceSchemaBase
		where TContext: ICreationContext
	{
		protected abstract TResult CreateObjectInternal(TContext context);

		protected virtual bool ValidateSchema(TContext context, out string error)
		{
			if (context == null)
			{
				error = "Schema cannot be null";
				return false;
			}
			if (!(context is TContext))
			{
				error = $"Incorrect context type. Expected: {typeof(TContext).FullName}\t Actual: {context.GetType().FullName}";
				return false;
			}
			return context.Verify(out error);
		}

		public IRESTResource CreateObject(ICreationContext context)
		{
			if (!ValidateSchema((TContext)context, out var error))
			{
				throw new Exception(error);
			}
			return CreateObjectInternal((TContext)context);
		}

		public TResult CreateTypedObject(ICreationContext context)
		{
			if(!(context is TContext))
			{
				throw new InvalidCastException($"Incorrect context: {context?.GetType()}");
			}
			var obj = CreateObject((TContext)context);
			if(!(obj is TResult typedObj))
			{
				throw new Exception($"Could not create object: Generated incorrect type {obj.GetType()}");
			}
			return typedObj;
		}

		public Type SchemaType => typeof(TSchema);
	}
}
