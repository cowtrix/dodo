using Dodo.LocationResources;
using DodoResources.Sites;

namespace Dodo.Controllers.Edit
{
	public class EventController : CrudController<Event, EventSchema>
	{
		protected override AuthorizationService<Event, EventSchema> AuthService =>
			new OwnedResourceAuthService<Event, EventSchema>();
	}
}
