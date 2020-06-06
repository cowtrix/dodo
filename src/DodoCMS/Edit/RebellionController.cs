using Dodo.Rebellions;
using Resources;

namespace Dodo.Controllers.Edit
{
	public class RebellionController : CrudController<Rebellion, RebellionSchema>
	{
		protected override PublicResourceAPIController<Rebellion, RebellionSchema> RESTController => 
			new DodoResources.Rebellions.RebellionController() as PublicResourceAPIController<Rebellion, RebellionSchema>;
	}
}
