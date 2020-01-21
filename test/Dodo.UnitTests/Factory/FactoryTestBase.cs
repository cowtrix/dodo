using Dodo;
using Dodo.SharedTest;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REST;
using SharedTest;
using System.Collections.Generic;
using System.Text;

namespace Factory
{
	public abstract class FactoryTestBase<T, TSchema> : TestBase
		where T : IRESTResource
		where TSchema : ResourceSchemaBase
	{
		[TestMethod]
		public void CanCreateFromSchema()
		{
			var factory = ResourceUtility.GetFactory<T>();
			var schema = (TSchema)SchemaGenerator.GetRandomSchema<T>(GetCreationContext());
			var newObj = factory.CreateObject(schema);
			VerifyCreatedObject(newObj, schema);
		}

		protected abstract AccessContext GetCreationContext();

		protected abstract void VerifyCreatedObject(T obj, TSchema schema);

		protected User GetRandomUser(out string password)
		{
			var schema = (UserSchema)SchemaGenerator.GetRandomSchema<User>(default);
			password = schema.Password;
			return ResourceUtility.GetFactory<User>().CreateObject(schema);
		}
	}
}
