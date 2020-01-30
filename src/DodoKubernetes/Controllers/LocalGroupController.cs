using REST;
using Microsoft.AspNetCore.Mvc;

namespace Dodo.LocalGroups
{
	[Route(RootURL)]
	public class LocalGroupController : GroupResourceController<LocalGroup, LocalGroupSchema>
	{
		public const string RootURL = "api/localgroups";
	}
}
