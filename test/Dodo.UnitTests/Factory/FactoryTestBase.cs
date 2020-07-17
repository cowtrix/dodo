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
using Dodo.Users.Tokens;

namespace Factory
{
	public abstract class FactoryTestBase<T, TSchema> : TestBase
		where T : IRESTResource
		where TSchema : ResourceSchemaBase
	{
		[TestMethod]
		public void CanCreateFromSchema()
		{
			var factory = ResourceUtility.GetFactory(typeof(T));
			GetRandomUser(out _, out var context);
			var schema = (TSchema)SchemaGenerator.GetRandomSchema<T>(context);
			var req = new ResourceCreationRequest(context, schema);
			var newObj = (T)factory.CreateObject(req);
			VerifyCreatedObject(newObj, schema);
		}

		[TestMethod]
		public virtual void CannotCreateWithBadAuth()
		{
			var factory = ResourceUtility.GetFactory<T>();
			GetRandomUser(out _, out var context);
			var schema = (TSchema)SchemaGenerator.GetRandomSchema<T>(context) as ResourceSchemaBase;
			var badContext = new AccessContext(context.User, new Resources.Security.Passphrase("asbasdbasdb"));
			AssertX.Throws<Exception>(() => factory.CreateObject(new ResourceCreationRequest(badContext, schema)),
				e => e.Message.Contains("Bad authorisation"));
		}

		[TestMethod]
		public virtual void CannotCreateWithBadName()
		{
			var factory = ResourceUtility.GetFactory<T>();
			GetRandomUser(out _, out var context);
			var schema = (TSchema)SchemaGenerator.GetRandomSchema<T>(context) as ResourceSchemaBase;
			schema.Name = "$";
			AssertX.Throws<Exception>(() => factory.CreateObject(new ResourceCreationRequest(context, schema)),
				e => e.Message.Contains("Name length must be between"));
		}

		protected virtual void VerifyCreatedObject(T obj, TSchema schema)
		{
			Assert.IsTrue(obj.Verify(out var error), error);
			if(obj is ITokenResource token)
			{
				Assert.IsNotNull(token.PublicKey);
			}
		}
	}
}
