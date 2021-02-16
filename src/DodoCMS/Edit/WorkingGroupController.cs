using Dodo.ViewModels;
using Dodo.WorkingGroups;
using Dodo;

namespace Dodo.Controllers.Edit
{
	public class WorkingGroupController : AdministratedGroupResourceCrudController<WorkingGroup, WorkingGroupSchema, WorkingGroupViewModel>
	{
		protected override AuthorizationService<WorkingGroup, WorkingGroupSchema> AuthService =>
			new AdministratedGroupResourceAuthService<WorkingGroup, WorkingGroupSchema>();
	}
}
