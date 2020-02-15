using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace Resources.Serializers
{
	public abstract class ResourceReferenceSerializer<T> : IBsonSerializer<ResourceReference<T>>, ICustomBsonSerializer where T: class, IRESTResource
	{
		public Type ValueType => typeof(ResourceReference<T>);

		public ResourceReference<T> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			Guid guid = context.Reader.ReadBinaryData().AsGuid;
			return new ResourceReference<T>(guid);
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ResourceReference<T> value)
		{
			context.Writer.WriteBinaryData(value.Guid);
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
		{
			Serialize(context, args, (ResourceReference<T>)value);
		}

		object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			return Deserialize(context, args);
		}
	}
}
