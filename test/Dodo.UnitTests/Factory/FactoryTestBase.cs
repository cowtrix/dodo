using Dodo;
using Dodo.SharedTest;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using SharedTest;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Extensions;

namespace Factory
{
	public abstract class FactoryTestBase<T, TSchema> : TestBase
		where T : IRESTResource
		where TSchema : DodoResourceSchemaBase
	{
		[TestMethod]
		public void CanCreateFromSchema()
		{
			var factory = ResourceUtility.GetFactory(typeof(T));
			GetRandomUser(out _, out var context);
			var schema = (TSchema)SchemaGenerator.GetRandomSchema<T>(context);
			var newObj = (T)factory.CreateObject(context, schema);
			VerifyCreatedObject(newObj, schema);
		}

		[TestMethod]
		public virtual void CannotCreateWithBadAuth()
		{
			var factory = ResourceUtility.GetFactory<T>();
			GetRandomUser(out _, out var context);
			var schema = (TSchema)SchemaGenerator.GetRandomSchema<T>(context) as DodoResourceSchemaBase;
			var badContext = new AccessContext(context.User, new Resources.Security.Passphrase("asbasdbasdb"));
			AssertX.Throws<Exception>(() => factory.CreateObject(badContext, schema),
				e => e.Message.Contains("Bad authorisation"));
		}


		[TestMethod]
		public virtual void CannotCreateWithBadName()
		{
			var factory = ResourceUtility.GetFactory<T>();
			GetRandomUser(out _, out var context);
			var schema = (TSchema)SchemaGenerator.GetRandomSchema<T>(context) as DodoResourceSchemaBase;
			schema.Name = "Admin - this is an invalid name value";
			AssertX.Throws<Exception>(() => factory.CreateObject(context, schema),
				e => e.Message.Contains("Name contains reserved word"));
		}

		protected virtual void VerifyCreatedObject(T obj, TSchema schema)
		{
			Assert.IsTrue(obj.Verify(out var error), error);
		}
	}
}
