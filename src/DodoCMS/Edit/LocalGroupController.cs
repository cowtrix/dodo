using Dodo.LocalGroups;
using Dodo.ViewModels;
using DodoResources;

namespace Dodo.Controllers.Edit
{
	public class LocalGroupController : CrudController<LocalGroup, LocalGroupSchema, LocalGroupViewModel>
	{
		protected override AuthorizationService<LocalGroup, LocalGroupSchema> AuthService =>
			new GroupResourceAuthService<LocalGroup, LocalGroupSchema>();
	}
}
