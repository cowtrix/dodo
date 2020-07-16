using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Resources.Location;
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
			context.Reader.ReadName();
			var typeName = context.Reader.ReadString();
			Type type = Type.GetType(typeName);
			context.Reader.ReadName();
			string name = context.Reader.ReadString();
			context.Reader.ReadName();
			var latitude = context.Reader.ReadDouble();
			context.Reader.ReadName();
			var longitude = context.Reader.ReadDouble();			
			GeoLocation loc = null;
			if(latitude != 0 && latitude != 0)
			{
				loc = new GeoLocation(latitude, longitude);
			}
			context.Reader.ReadName();
			var parent = context.Reader.ReadBinaryData().AsGuid;
			context.Reader.ReadName();
			var desc = context.Reader.ReadString();
			context.Reader.ReadEndDocument();
			return new ResourceReference<T>(guid, slug, type, name, loc, parent, desc);
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ResourceReference<T> value)
		{
			context.Writer.WriteStartDocument();
			context.Writer.WriteName(nameof(value.Guid));
			context.Writer.WriteBinaryData(value.Guid);
			context.Writer.WriteName(nameof(value.Slug));
			context.Writer.WriteString(value.Slug ?? string.Empty);
			context.Writer.WriteName(nameof(value.FullyQualifiedName));
			context.Writer.WriteString(value.FullyQualifiedName ?? string.Empty);
			context.Writer.WriteName(nameof(value.Name));
			context.Writer.WriteString(value.Name ?? string.Empty);
			context.Writer.WriteName(nameof(value.Location.Latitude));
			context.Writer.WriteDouble(value.Location != null ? value.Location.Latitude : 0);
			context.Writer.WriteName(nameof(value.Location.Longitude));
			context.Writer.WriteDouble(value.Location != null ? value.Location.Longitude : 0);
			context.Writer.WriteName(nameof(value.Guid));
			context.Writer.WriteBinaryData(value.Parent);
			context.Writer.WriteName(nameof(value.PublicDescription));
			context.Writer.WriteString(value.PublicDescription ?? string.Empty);
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
