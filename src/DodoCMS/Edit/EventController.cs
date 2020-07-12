using Dodo.LocationResources;
using Dodo.ViewModels;
using Dodo.Sites;

namespace Dodo.Controllers.Edit
{
	public class EventController : CrudController<Event, EventSchema, EventViewModel>
	{
		protected override AuthorizationService<Event, EventSchema> AuthService =>
			new OwnedResourceAuthService<Event, EventSchema>();
	}
}
