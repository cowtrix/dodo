using Dodo.Rebellions;
using Resources;

namespace Dodo.Controllers.Edit
{
	public class RebellionController : CrudController<Rebellion, RebellionSchema>
	{
		protected override ResourceController<Rebellion, RebellionSchema> RESTController => 
			new DodoResources.Rebellions.RebellionController() as ResourceController<Rebellion, RebellionSchema>;
	}
}
