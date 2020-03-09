using Common.Extensions;
using Dodo.Resources;
using Resources;
using Resources.Serializers;
using System;

namespace Dodo.Users
{
	public class UserSerializer : ResourceReferenceSerializer<User> { }

	public class UserSchema : DodoResourceSchemaBase
	{
		[Username]
		public string Username { get; set; }
		[Password]
		public string Password { get; set; }
		[Email]
		public string Email { get; set; }

		public UserSchema(string name, string username, string password, string email) : base(name)
		{
			Username = username;
			Password = password;
			Email = email;
		}

		public UserSchema()
		{
		}
	}

	public class UserFactory : DodoResourceFactory<User, UserSchema>
	{
		protected override bool ValidateSchema(AccessContext context, ResourceSchemaBase schema, out string error)
		{
			if (schema == null)
			{
				throw new NullReferenceException("Schema cannot be null");
			}
			if (!(schema is UserSchema))
			{
				throw new InvalidCastException($"Incorrect schema type. Expected: {typeof(UserSchema).FullName}\t Actual: {schema.GetType().FullName}");
			}
			error = null;
			return true;
		}

		protected override User CreateObjectInternal(AccessContext context, UserSchema schema)
		{
			var user = base.CreateObjectInternal(context, schema);
			user.TokenCollection.Add(user, new VerifyEmailToken());
			return user;
		}
	}
}
