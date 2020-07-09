using Common.Extensions;
using Dodo.DodoResources;
using Dodo.Users.Tokens;
using Resources;
using Resources.Serializers;
using System;
using System.ComponentModel;

namespace Dodo.Users
{
	public class UserSerializer : ResourceReferenceSerializer<User> { }

	public class UserSchema : ResourceSchemaBase
	{
		[Slug]
		public string Username { get; set; }
		[Password]
		[PasswordPropertyText]
		public string Password { get; set; }
		[Email]
		public string Email { get; set; }
		
		public string Token { get; set; }

		public UserSchema(string name, string username, string password, string email, string token = null) : base(name)
		{
			Username = username;
			Password = password;
			Email = email;
			Token = token;
		}

		public UserSchema()
		{
		}
	}

	public class UserFactory : DodoResourceFactory<User, UserSchema>
	{
		protected override bool ValidateSchema(ResourceCreationRequest request, out string error)
		{
			if (request.Schema == null)
			{
				throw new NullReferenceException("Schema cannot be null");
			}
			if (!(request.Schema is UserSchema))
			{
				throw new InvalidCastException($"Incorrect schema type. Expected: {typeof(UserSchema).FullName}\t Actual: {request.Schema.GetType().FullName}");
			}
			return request.Verify(out error);
		}

		protected override User CreateObjectInternal(ResourceCreationRequest request)
		{
			var user = base.CreateObjectInternal(request);
			using var rscLock = new ResourceLock(user);
			user.TokenCollection.Add(user, new VerifyEmailToken());
			ResourceUtility.GetManager<User>().Update(user, rscLock);
			return user;
		}
	}
}
