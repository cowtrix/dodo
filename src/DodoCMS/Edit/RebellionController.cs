using Dodo.Rebellions;
using DodoResources;
using Resources;

namespace Dodo.Controllers.Edit
{
	public class RebellionController : CrudController<Rebellion, RebellionSchema>
	{
		protected override AuthorizationService<Rebellion, RebellionSchema> AuthService =>
			new GroupResourceAuthService<Rebellion, RebellionSchema>();
	}
}
