using Microsoft.AspNetCore.Mvc;

namespace Dodo
{
	public abstract class GroupResourceAPIController<T, TSchema> : SearchableResourceController<T, TSchema>
		where T : class, IGroupResource
		where TSchema : DescribedResourceSchemaBase
	{
		protected GroupResourceService<T, TSchema> GroupService =>
			new GroupResourceService<T, TSchema>(Context, HttpContext, new GroupResourceAuthService<T, TSchema>());
		protected override AuthorizationService<T, TSchema> AuthService => new GroupResourceAuthService<T, TSchema>();

		[HttpPost("{id}/" + IGroupResource.JOIN_GROUP)]
		public IActionResult JoinGroup(string id)
		{
			var result = GroupService.JoinGroup(id);
			return result.ActionResult;
		}

		[HttpPost("{id}/" + IGroupResource.LEAVE_GROUP)]
		public IActionResult LeaveGroup(string id)
		{
			var result = GroupService.LeaveGroup(id);
			return result.ActionResult;
		}
	}
}
