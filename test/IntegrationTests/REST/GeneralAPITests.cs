using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RESTTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTTests
{
	[TestClass]
	public class GeneralAPITests : IntegrationTestBase
	{
		[TestMethod]
		public async Task CanGetAPIEndpoint()
		{
			var request = await RequestJSON(Dodo.DodoApp.API_ROOT, Resources.EHTTPRequestType.GET);
			// Simple enough to just check that it has some resource types defined
			var rscTypes = request.Value<JArray>("resourceTypes").Values<JToken>().ToList();
			Assert.IsTrue(rscTypes.Any());
		}
	}
}
