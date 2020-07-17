using Dodo.Users;
using Dodo;
using System;

namespace Dodo.Controllers.Edit
{

	public abstract class GroupResourceCrudController<T, TSchema, TViewModel> : CrudController<T, TSchema, TViewModel>
		where T : class, IGroupResource
		where TSchema : DescribedResourceSchemaBase, new()
		where TViewModel : class, IViewModel, new()
	{
		protected GroupResourceService<T, TSchema> GroupService =>
			new GroupResourceService<T, TSchema>(Context, HttpContext, AuthService);
	}
}
