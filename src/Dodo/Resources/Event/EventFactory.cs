using Dodo.DodoResources;
using Resources.Serializers;

namespace Dodo.LocationResources
{
	public class EventSerializer : ResourceReferenceSerializer<Event> { }
	public class EventFactory : DodoResourceFactory<Event, EventSchema>
	{
	}
}
