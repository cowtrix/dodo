using Common;
using Dodo.Resources;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dodo.LocalGroups
{
	public class LocalGroupRESTHandler : DodoRESTHandler<LocalGroup>
	{
		const string CREATION_URL = "newlocalgroup";

		public class CreationSchema : IRESTResourceSchema
		{
			public string Name = "";
			public GeoLocation Location = new GeoLocation();
		}

		[Route("Create a new local group", "^" + CREATION_URL, EHTTPRequestType.POST)]
		public HttpResponse CreateLocalGroup(HttpRequest request)
		{
			return CreateObject(request);
		}

		[Route("List all local groups", "^localgroups$", EHTTPRequestType.GET)]
		public HttpResponse List(HttpRequest request)
		{
			var owner = DodoRESTServer.GetRequestOwner(request, out var passPhrase);
			return HttpBuilder.OK(DodoServer.ResourceManager<LocalGroup>().Get(x => true).ToList()
				.GenerateJsonView(EPermissionLevel.USER, owner, passPhrase));
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema();
		}

		protected override LocalGroup CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var user = DodoRESTServer.GetRequestOwner(request);
			if(user == null)
			{
				throw HTTPException.LOGIN;
			}
			var localGroup = new LocalGroup(user, info.Name, info.Location);
			DodoServer.ResourceManager<LocalGroup>().Add(localGroup);
			return localGroup;
		}
	}
}
