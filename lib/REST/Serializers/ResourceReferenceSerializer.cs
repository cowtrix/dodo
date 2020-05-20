using MongoDB.Bson;
using MongoDB.Bson.IO;
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
			context.Reader.ReadStartDocument();
			context.Reader.ReadName();
			Guid guid = context.Reader.ReadBinaryData().AsGuid;
			context.Reader.ReadName();
			string slug = context.Reader.ReadString();
			context.Reader.ReadEndDocument();
			return new ResourceReference<T>(guid, slug);
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ResourceReference<T> value)
		{
			context.Writer.WriteStartDocument();
			context.Writer.WriteName(nameof(value.Guid));
			context.Writer.WriteBinaryData(value.Guid);
			context.Writer.WriteName(nameof(value.Slug));
			context.Writer.WriteString(value.Slug ?? string.Empty);
			context.Writer.WriteEndDocument();
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
