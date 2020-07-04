using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using System;

namespace Dodo.Users.Tokens
{
	/// <summary>
	/// This token entitles the bearer to create 1 new object of the type
	/// specified in the Type field
	/// </summary>
	public class ResourceCreationToken : RedeemableToken
	{
		[JsonProperty]
		[BsonElement]
		public string ResourceType { get; private set; }

		[JsonIgnore]
		[BsonIgnore]
		public override bool Encrypted => true;

		public ResourceReference<IRESTResource> Target { get; set; }

		public ResourceCreationToken() { }

		public ResourceCreationToken(Type type)
		{
			if (!typeof(DodoResource).IsAssignableFrom(type))
			{
				throw new Exception($"Unexpected type: {type}");
			}
			ResourceType = type.Name;
		}

		public override EPermissionLevel GetVisibility() => EPermissionLevel.OWNER;
	}
}
