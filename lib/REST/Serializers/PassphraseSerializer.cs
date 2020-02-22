using Resources.Security;
using MongoDB.Bson.Serialization;
using System;

namespace Resources.Serializers
{
	public class PassphraseSerializer : IBsonSerializer<Passphrase>, ICustomBsonSerializer
	{
		public Type ValueType => typeof(Passphrase);

		public Passphrase Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			context.Reader.ReadStartDocument();
			var tokenKey = context.Reader.ReadString();
			context.Reader.ReadEndDocument();
			return new Passphrase { TokenKey = tokenKey };
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Passphrase value)
		{
			context.Writer.WriteStartDocument();
			context.Writer.WriteName("TokenKey");
			context.Writer.WriteString(value.TokenKey);
			context.Writer.WriteEndDocument();
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
