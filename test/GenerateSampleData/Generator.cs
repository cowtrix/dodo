using Dodo;
using Dodo.Rebellions;
using Dodo.SharedTest;
using Dodo.Users;
using REST;
using REST.Security;
using System;

namespace GenerateSampleData
{
	class Generator
	{
		static UserFactory UserFactory = new UserFactory();
		static RebellionFactory RebellionFactory = new RebellionFactory();

		static void Main(string[] args)
		{
			ResourceUtility.ClearAllManagers();
			GenerateRebellion();
		}

		private static void GenerateRebellion()
		{
			var rebellionCreatorSchema = SchemaGenerator.GetRandomUser(default);
			var rebellionCreator = UserFactory.CreateObject(rebellionCreatorSchema);
			var cntxt = new AccessContext(rebellionCreator, new Passphrase(rebellionCreatorSchema.Password));
			var rebellionSchema = SchemaGenerator.GetRandomRebellion(cntxt);
			var rebellion = RebellionFactory.CreateObject(rebellionSchema);

		}
	}
}
