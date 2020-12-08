using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Resources.Location;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Resources.Serializers
{
	public abstract class ResourceReferenceSerializer<T> : IBsonSerializer<ResourceReference<T>>, ICustomBsonSerializer where T : class, IRESTResource
	{
		static Dictionary<string, string> m_fieldMapping = new Dictionary<string, string>()
		{
			{ nameof(IResourceReference.Name),                  "N" },
			{ nameof(IResourceReference.Guid),                  "G" },
			{ nameof(IResourceReference.Parent),                "P" },
			{ nameof(IResourceReference.Slug),                  "S" },
			{ nameof(IResourceReference.FullyQualifiedName),    "F" },
			{ nameof(IResourceReference.PublicDescription),     "D" },
			{ nameof(IResourceReference.Location),              "L" },
			{ nameof(IResourceReference.Location.Longitude),    "l" },
			{ nameof(IResourceReference.IsPublished),           "I" },
		};
		static Dictionary<string, string> m_reverseFieldMapping = m_fieldMapping.ToDictionary(k => k.Value, k => k.Key);

		public Type ValueType => typeof(ResourceReference<T>);

		public ResourceReference<T> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			Guid guid = default, parent = default;
			string name = default, slug = default, typeName = default, description = default;
			Type type = default;
			GeoLocation loc = default;
			bool isPublished = default;

			context.Reader.ReadStartDocument();
			string currentName;
			do
			{
				if (context.Reader.State == BsonReaderState.Type)
				{
					context.Reader.ReadBsonType();
				}
				currentName = LengthenName(context.Reader.ReadName());
				switch (currentName)
				{
					case nameof(IResourceReference.Guid):
						guid = context.Reader.ReadBinaryData().AsGuid;
						break;
					case nameof(IResourceReference.Parent):
						parent = context.Reader.ReadBinaryData().AsGuid;
						break;
					case nameof(IResourceReference.Name):
						name = context.Reader.ReadString();
						break;
					case nameof(IResourceReference.Slug):
						slug = context.Reader.ReadString();
						break;
					case nameof(IResourceReference.FullyQualifiedName):
						typeName = context.Reader.ReadString();
						type = Type.GetType(typeName);
						break;
					case nameof(IResourceReference.PublicDescription):
						description = context.Reader.ReadString();
						break;
					case nameof(IResourceReference.Location):
						var lat = context.Reader.ReadDouble();
						context.Reader.ReadName();
						var lng = context.Reader.ReadDouble();
						if (lat != 0 && lng != 0)
						{
							loc = new GeoLocation(lat, lng);
						}
						break;
					case nameof(IResourceReference.IsPublished):
						isPublished = context.Reader.ReadBoolean();
						break;
				}
			}
			while (currentName != nameof(IResourceReference.Guid));
			context.Reader.ReadEndDocument();
			return new ResourceReference<T>(guid, slug, type, name, loc, parent, description, isPublished);
		}

		static string ShortenName(string n)
		{
			return m_fieldMapping[n];
		}

		static string LengthenName(string n)
		{
			return m_reverseFieldMapping[n];
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ResourceReference<T> value)
		{
			context.Writer.WriteStartDocument();
			if (!string.IsNullOrEmpty(value.Name))
			{
				context.Writer.WriteName(ShortenName(nameof(value.Name)));
				context.Writer.WriteString(value.Name);
			}
			if (!string.IsNullOrEmpty(value.Slug))
			{
				context.Writer.WriteName(ShortenName(nameof(value.Slug)));
				context.Writer.WriteString(value.Slug);
			}
			if (!string.IsNullOrEmpty(value.PublicDescription))
			{
				context.Writer.WriteName(ShortenName(nameof(value.PublicDescription)));
				context.Writer.WriteString(value.PublicDescription);
			}
			if (!string.IsNullOrEmpty(value.FullyQualifiedName))
			{
				context.Writer.WriteName(ShortenName(nameof(value.FullyQualifiedName)));
				context.Writer.WriteString(value.FullyQualifiedName);
			}
			if (value.Location != null)
			{
				context.Writer.WriteName(ShortenName(nameof(value.Location)));
				context.Writer.WriteDouble(value.Location.Latitude);
				context.Writer.WriteName(ShortenName(nameof(value.Location.Longitude)));
				context.Writer.WriteDouble(value.Location.Longitude);
			}
			if (typeof(IPublicResource).IsAssignableFrom(value.GetRefType()))
			{
				context.Writer.WriteName(ShortenName(nameof(value.IsPublished)));
				context.Writer.WriteBoolean(value.IsPublished);
			}
			if (value.Parent != default)
			{
				context.Writer.WriteName(ShortenName(nameof(value.Parent)));
				context.Writer.WriteBinaryData(value.Parent);
			}
			context.Writer.WriteName(ShortenName(nameof(value.Guid)));
			context.Writer.WriteBinaryData(value.Guid);
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
