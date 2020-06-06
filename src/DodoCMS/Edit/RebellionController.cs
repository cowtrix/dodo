using Dodo.Rebellions;
using Resources;

namespace Dodo.Controllers.Edit
{
	public class RebellionController : CrudController<Rebellion, RebellionSchema>
	{
		protected override CrudResourceAPIController<Rebellion, RebellionSchema> RESTController => 
			new DodoResources.Rebellions.RebellionAPIController() as CrudResourceAPIController<Rebellion, RebellionSchema>;
	}
}
