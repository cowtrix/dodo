using Dodo.WorkingGroups;
using DodoResources;

namespace Dodo.Controllers.Edit
{
	public class WorkingGroupController : CrudController<WorkingGroup, WorkingGroupSchema>
	{
		protected override AuthorizationService<WorkingGroup, WorkingGroupSchema> AuthService =>
			new GroupResourceAuthService<WorkingGroup, WorkingGroupSchema>();
	}
}
