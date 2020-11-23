using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources.Security;
using Resources.Serializers;
using System;

namespace Resources
{
	public struct HashedResourceReference
	{
		[JsonProperty]
		public string Key { get; set; }

		public HashedResourceReference(IRESTResource rsc, string salt)
		{
			if (string.IsNullOrEmpty(salt))
			{
				Key = null;
				return;
			}
			Key = GetKey(rsc, salt);
		}

		public bool IsResource(IRESTResource rsc, string salt)
		{
			if (string.IsNullOrEmpty(Key))
			{
				return false;
			}
			return GetKey(rsc, salt) == Key;
		}

		private static string GetKey(IRESTResource rsc, string salt)
			=> SecurityExtensions.GenerateID(rsc, salt);

		public override bool Equals(object obj)
		{
			return obj is HashedResourceReference reference &&
				   Key == reference.Key;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Key);
		}
	}

	public class HashedResourceReferenceSerialzer :
		IBsonSerializer<HashedResourceReference>, ICustomBsonSerializer
	{
		public Type ValueType => typeof(HashedResourceReference);

		public HashedResourceReference Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			context.Reader.ReadStartDocument();
			context.Reader.ReadName();
			var key = context.Reader.ReadString();
			context.Reader.ReadEndDocument();
			return new HashedResourceReference { Key = key };
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, HashedResourceReference value)
		{
			context.Writer.WriteStartDocument();
			context.Writer.WriteName(nameof(value.Key));
			context.Writer.WriteString(value.Key ?? "");
			context.Writer.WriteEndDocument();
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
		{
			Serialize(context, args, (HashedResourceReference)value);
		}

		object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			return Deserialize(context, args);
		}
	}
}
