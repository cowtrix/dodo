using Dodo.LocalGroups;
using DodoResources;

namespace Dodo.Controllers.Edit
{
	public class LocalGroupController : CrudController<LocalGroup, LocalGroupSchema>
	{
		protected override AuthorizationService<LocalGroup, LocalGroupSchema> AuthService =>
			new GroupResourceAuthService<LocalGroup, LocalGroupSchema>();
	}
}
