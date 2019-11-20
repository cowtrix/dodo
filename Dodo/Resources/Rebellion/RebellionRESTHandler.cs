using Common;
using Dodo.Resources;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dodo.Rebellions
{
	public class RebellionRESTHandler : DodoRESTHandler<Rebellion>
	{
		public class CreationSchema : IRESTResourceSchema
		{
			public string RebellionName = "";
			public GeoLocation Location = new GeoLocation();
		}

		[Route("Create a new rebellion", "newrebellion", EHTTPRequestType.POST)]
		public HttpResponse Register(HttpRequest request)
		{
			return CreateObject(request);
		}

		[Route("List all rebellions", "^rebellions$", EHTTPRequestType.GET)]
		public HttpResponse List(HttpRequest request)
		{
			var owner = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			return HttpBuilder.OK(DodoServer.ResourceManager<Rebellion>().Get(x => true).ToList()
				.GenerateJsonView(EPermissionLevel.USER, owner, passphrase));
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema();
		}

		protected override Rebellion CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var user = DodoRESTServer.GetRequestOwner(request);
			if(user == null)
			{
				throw HTTPException.LOGIN;
			}
			var newRebellion = new Rebellion(user, info.RebellionName, info.Location);
			DodoServer.ResourceManager<Rebellion>().Add(newRebellion);
			return newRebellion;
		}
	}
}
