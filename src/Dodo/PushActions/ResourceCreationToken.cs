using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Dodo.Users
{

	public class ResourceCreationToken : OneTimeRedeemableToken
	{
		[BsonElement]
		public string Type { get; private set; }

		public ResourceCreationToken(Type type)
		{
			if(!typeof(DodoResource).IsAssignableFrom(type))
			{
				throw new Exception($"Unexpected type: {type}");
			}
			Type = type.Name;
		}
	}
}
