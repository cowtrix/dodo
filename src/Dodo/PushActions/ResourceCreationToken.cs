using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Dodo.Users
{

	public class ResourceCreationToken : UserToken
	{
		public bool Redeemed { get; set; }
		[BsonElement]
		public Type Type { get; private set; }

		public ResourceCreationToken(Type type)
		{
			if(!typeof(DodoResource).IsAssignableFrom(type))
			{
				throw new Exception($"Unexpected type: {type}");
			}
		}
	}
}
