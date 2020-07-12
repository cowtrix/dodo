using Dodo.LocalGroups;
using Dodo.ViewModels;
using Dodo;

namespace Dodo.Controllers.Edit
{
	public class LocalGroupController : GroupResourceCrudController<LocalGroup, LocalGroupSchema, LocalGroupViewModel>
	{
		protected override AuthorizationService<LocalGroup, LocalGroupSchema> AuthService =>
			new GroupResourceAuthService<LocalGroup, LocalGroupSchema>();
	}
}
