using Dodo.LocalGroups;
using Dodo.LocationResources;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.WorkingGroups;
using DodoResources;
using DodoResources.Roles;
using DodoResources.Sites;
using Resources;

namespace Dodo.Controllers.Edit
{
	public class RebellionController : CrudController<Rebellion, RebellionSchema>
	{
		protected override AuthorizationService<Rebellion, RebellionSchema> AuthService =>
			new GroupResourceAuthService<Rebellion, RebellionSchema>();
	}

	public class WorkingGroupController : CrudController<WorkingGroup, WorkingGroupSchema>
	{
		protected override AuthorizationService<WorkingGroup, WorkingGroupSchema> AuthService =>
			new GroupResourceAuthService<WorkingGroup, WorkingGroupSchema>();
	}

	public class LocalGroupController : CrudController<LocalGroup, LocalGroupSchema>
	{
		protected override AuthorizationService<LocalGroup, LocalGroupSchema> AuthService =>
			new GroupResourceAuthService<LocalGroup, LocalGroupSchema>();
	}

	public class EventController : CrudController<Event, EventSchema>
	{
		protected override AuthorizationService<Event, EventSchema> AuthService =>
			new OwnedResourceAuthService<Event, EventSchema>();
	}

	public class SiteController : CrudController<Site, SiteSchema>
	{
		protected override AuthorizationService<Site, SiteSchema> AuthService =>
			new OwnedResourceAuthService<Site, SiteSchema>();
	}

	public class RoleController : CrudController<Role, RoleSchema>
	{
		protected override AuthorizationService<Role, RoleSchema> AuthService =>
			new RoleAuthService();
	}
}
