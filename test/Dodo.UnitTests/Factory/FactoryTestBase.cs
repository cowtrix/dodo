using Dodo;
using Dodo.SharedTest;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REST;
using SharedTest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Factory
{
	public abstract class FactoryTestBase<T, TSchema> : TestBase
		where T : IRESTResource
		where TSchema : DodoResourceSchemaBase
	{
		[TestMethod]
		public void CanCreateFromSchema()
		{
			var factory = ResourceUtility.GetFactory<T>();
			var schema = (TSchema)SchemaGenerator.GetRandomSchema<T>(GetCreationContext());
			var newObj = factory.CreateObject(schema);
			VerifyCreatedObject(newObj, schema);
		}

		[TestMethod]
		public virtual void CannotCreateWithBadAuth()
		{
			var factory = ResourceUtility.GetFactory<T>();
			var context = GetCreationContext();
			var schema = (TSchema)SchemaGenerator.GetRandomSchema<T>(context) as DodoResourceSchemaBase;
			schema.Context = new AccessContext(context.User, new REST.Security.Passphrase("asbasdbasdb"));
			AssertX.Throws<Exception>(() => factory.CreateObject(schema),
				e => e.Message.Contains("Bad Context"));
		}


		[TestMethod]
		public virtual void CannotCreateWithBadName()
		{
			var factory = ResourceUtility.GetFactory<T>();
			var context = GetCreationContext();
			var schema = (TSchema)SchemaGenerator.GetRandomSchema<T>(context) as DodoResourceSchemaBase;
			schema.Name = "Admin - this is an invalid name value";
			AssertX.Throws<Exception>(() => factory.CreateObject(schema),
				e => e.Message.Contains("Name contains reserved word"));
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
