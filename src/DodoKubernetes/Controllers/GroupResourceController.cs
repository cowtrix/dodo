using System.Collections.Generic;
using Common;
using Common.Extensions;
using REST.Security;
using Dodo.Users;
using Dodo.Utility;
using Newtonsoft.Json;
using REST;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Common.Security;
using System.Net;
using System.Linq;

namespace Dodo
{
	public abstract class GroupResourceController<T, TSchema> : ObjectRESTController<T, TSchema> 
		where T : GroupResource
		where TSchema : DodoResourceSchemaBase
	{
		public const string ADD_ADMIN = "?addadmin";
		public const string JOIN_GROUP = "?join";
		public const string LEAVE_GROUP = "?leave";

		private bool IsResourceActionUrl(string url, string postfix)
		{
			if (!url.EndsWith(postfix))
			{
				return false;
			}
			url = url.Substring(0, url.Length - postfix.Length);
			var resource = ResourceUtility.GetResourceByURL(url) as GroupResource;
			if (resource == null)
			{
				return false;
			}
			return resource is T;
		}

		IActionResult AddAdministrator()
		{
			var target = ResourceUtility.GetResourceByURL(Request.Path) as T;
			if (target == null)
			{
				return NotFound();
			}
			var context = Request.GetRequestOwner();
			if (!IsAuthorised(context, target, Request.MethodEnum(), out var permissionLevel))
			{
				return Forbid();
			}
			if (permissionLevel < EPermissionLevel.ADMIN)
			{
				throw HttpException.FORBIDDEN;
			}
			var resourceUrl = Request.Path.Value?.Substring(0, Request.Path.Value.Length - ADD_ADMIN.Length);
			var resource = ResourceUtility.GetResourceByURL(resourceUrl) as GroupResource;
			if (resource == null)
			{
				return NotFound();
			}
			var targetEmail = JsonConvert.DeserializeObject<string>(Request.ReadBody());
			var targetUser = ResourceUtility.GetManager<User>().GetSingle(x => x.GUID.ToString() == targetEmail || x.Email == targetEmail);
			if(resource.AddAdmin(context, targetUser))
			{
				return Ok();
			}
			return BadRequest();
		}

		IActionResult JoinGroup()
		{
			var context = Request.GetRequestOwner();
			using (var resourceLock = new ResourceLock(Request.Path))
			{
				var target = resourceLock.Value as GroupResource;
				if (target == null)
				{
					return NotFound();
				}
				target.Members.Add(context.User, context.Passphrase);
				ResourceManager.Update(target, resourceLock);
				return Ok();
			}
		}

		IActionResult LeaveGroup()
		{
			var context = Request.GetRequestOwner();
			using (var resourceLock = new ResourceLock(Request.Path))
			{
				var target = resourceLock.Value as GroupResource;
				if (target == null)
				{
					return NotFound();
				}
				target.Members.Remove(context.User, context.Passphrase);
				ResourceManager.Update(target, resourceLock);
				return HttpBuilder.OK();
			}
		}

		[HttpGet]
		public IActionResult Index()
		{
			return Ok(ResourceManager.Get(x => true).Select(rsc => rsc.GUID));
		}

		protected override bool CanCreateAtUrl(AccessContext context, string url, out string error)
		{
			if (context.User == null)
			{
				error = "You need to login";
				return false;
			}
			if (context.User.EmailVerified)
			{
				error = "You need to verify your email";
				return false;
			}
			error = null;
			return true;
		}
	}
}
