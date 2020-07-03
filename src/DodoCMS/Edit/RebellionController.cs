using Dodo.Rebellions;
using Dodo.ViewModels;
using DodoResources;
using Resources;

namespace Dodo.Controllers.Edit
{
	public class RebellionController : GroupResourceCrudController<Rebellion, RebellionSchema, RebellionViewModel>
	{
		protected override AuthorizationService<Rebellion, RebellionSchema> AuthService =>
			new GroupResourceAuthService<Rebellion, RebellionSchema>();
	}
}
