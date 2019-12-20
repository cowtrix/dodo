using Common.Security;
using MongoDB.Bson.Serialization;
using System;

namespace SimpleHttpServer.REST.Serializers
{
	public abstract class PassphraseSerializer<T> : IBsonSerializer<Passphrase>, ICustomBsonSerializer
	{
		public Type ValueType => typeof(Passphrase);

		public Passphrase Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			var tokenKey = context.Reader.ReadString();
			var data = context.Reader.ReadString();
			return new Passphrase(tokenKey, data);
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Passphrase value)
		{
			context.Writer.WriteString(value.TokenKey);
			context.Writer.WriteString(value.Data);
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
		{
			Serialize(context, args, (Passphrase)value);
		}

		object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			return Deserialize(context, args);
		}
	}
}
