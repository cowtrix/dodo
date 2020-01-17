using REST;
using REST.Serializers;

namespace Dodo.Users
{
	public class UserSerializer : ResourceReferenceSerializer<User> { }

	public class UserSchema : DodoResourceSchemaBase
	{
		public string Username { get; private set; }
		public string Password { get; private set; }
		public string Email { get; private set; }
		public UserSchema(AccessContext context, string name, string username, string password, string email) : base(context, name)
		{
			Username = username;
			Password = password;
			Email = email;
		}
	}

	public class UserFactory : ResourceFactory<User, UserSchema>
	{
	}
}
