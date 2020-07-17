using Dodo.Rebellions;
using Dodo.ViewModels;
using Dodo;
using Resources;

namespace Dodo.Controllers.Edit
{
	public class RebellionController : AdministratedGroupResourceCrudController<Rebellion, RebellionSchema, RebellionViewModel>
	{
		protected override AuthorizationService<Rebellion, RebellionSchema> AuthService =>
			new AdministratedGroupResourceAuthService<Rebellion, RebellionSchema>();
	}
}
