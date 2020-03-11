using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Dodo.Users.Tokens
{
	/// <summary>
	/// This token entitles the bearer to create 1 new object of the type
	/// specified in the Type field
	/// </summary>
	public class ResourceCreationToken : RedeemableToken
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
