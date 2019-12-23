using Common;
using MongoDB.Bson.Serialization;
using System;

namespace SimpleHttpServer.REST.Serializers
{
	public class GeoLocationSerializer : IBsonSerializer<GeoLocation>, ICustomBsonSerializer
	{
		public Type ValueType => typeof(GeoLocation);

		public GeoLocation Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			context.Reader.ReadStartDocument();
			var lat = context.Reader.ReadDouble();
			var lo = context.Reader.ReadDouble();
			context.Reader.ReadEndDocument();
			return new GeoLocation(lat, lo);
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, GeoLocation value)
		{
			context.Writer.WriteStartDocument();
			context.Writer.WriteName("Latitude");
			context.Writer.WriteDouble(value.Latitude);
			context.Writer.WriteName("Longitude");
			context.Writer.WriteDouble(value.Longitude);
			context.Writer.WriteEndDocument();
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
		{
			Serialize(context, args, (GeoLocation)value);
		}

		object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			return Deserialize(context, args);
		}
	}
}
