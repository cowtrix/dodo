﻿using Dodo.Resources;
using REST;
using REST.Serializers;
using System;

namespace Dodo.Users
{
	public class UserSerializer : ResourceReferenceSerializer<User> { }

	public class UserSchema : DodoResourceSchemaBase
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public UserSchema(AccessContext context, string name, string username, string password, string email) : base(context, name)
		{
			Username = username;
			Password = password;
			Email = email;
		}

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
		protected override bool ValidateSchema(ResourceSchemaBase schema, out string error)
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
	}
}
