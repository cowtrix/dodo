using System.Collections.Generic;
using Common;
using Common.Extensions;
using Resources.Security;
using Dodo.Users;
using Dodo.Utility;
using Newtonsoft.Json;
using Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Common.Security;
using System.Net;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using Dodo;
using GeoCoordinatePortable;

namespace DodoResources
{
	public abstract class GroupResourceController<T, TSchema> : ObjectRESTController<T, TSchema> 
		where T : GroupResource
		where TSchema : DodoResourceSchemaBase
	{
		public const string ADD_ADMIN = "addadmin";
		public const string JOIN_GROUP = "join";
		public const string LEAVE_GROUP = "leave";

		public GroupResourceController(IAuthorizationService authorizationService) : base(authorizationService)
		{
		}

		[HttpPost("{id}/" + ADD_ADMIN)]
		[Authorize]
		public IActionResult AddAdministrator(Guid resourceID, [FromBody]string newAdminIdentifier)
		{
			var resource = ResourceManager.GetSingle(x => x.GUID == resourceID);
			if (resource == null)
			{
				return NotFound();
			}
			var context = User.GetRequestOwner();
			if (!IsAuthorised(context, resource, Request.MethodEnum(), out var permissionLevel))
			{
				return Forbid();
			}
			if (permissionLevel < EPermissionLevel.ADMIN)
			{
				return Unauthorized();
			}
			var userManager = ResourceUtility.GetManager<User>();
			User targetUser = null;
			if(Guid.TryParse(newAdminIdentifier, out var newAdminGuid))
			{
				targetUser = userManager.GetSingle(x => x.GUID == newAdminGuid);
			}
			else if(ValidationExtensions.EmailIsValid(newAdminIdentifier))
			{
				targetUser = userManager.GetSingle(x => x.PersonalData.Email == newAdminIdentifier);
			}
			if(resource.AddAdmin(context, targetUser))
			{
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("{id}/" + JOIN_GROUP)]
		[Authorize]
		public IActionResult JoinGroup(Guid id)
		{
			var context = User.GetRequestOwner();
			using var resourceLock = new ResourceLock(id);
			var target = resourceLock.Value as GroupResource;
			if (target == null)
			{
				return NotFound();
			}
			target.Members.Add(context.User, context.Passphrase);
			ResourceManager.Update(target, resourceLock);
			return Ok();
		}

		[HttpPost("{id}/" + LEAVE_GROUP)]
		[Authorize]
		public IActionResult LeaveGroup(Guid id)
		{
			var context = User.GetRequestOwner();
			using var resourceLock = new ResourceLock(id);
			var target = resourceLock.Value as GroupResource;
			if (target == null)
			{
				return NotFound();
			}
			target.Members.Remove(context.User, context.Passphrase);
			ResourceManager.Update(target, resourceLock);
			return Ok();
		}

		public const char FilterVarSeperatorChar = '+';
		public class ResourceFilterModel<T>
		{
			public string latlong;
			public double? distance;
			public string startdate;
			public string enddate;

			private bool m_generatedData;
			private GeoCoordinate m_coordinate;
			private DateTime m_startDate;
			private DateTime m_endDate;

			public void GenerateFilterData()
			{
				if(m_generatedData)
				{
					return;
				}
				m_generatedData = true;
				m_coordinate = latlong.Split(FilterVarSeperatorChar).Select(x => double.Parse(x))
							.Transpose(x => new GeoCoordinate(x.ElementAt(0), x.ElementAt(1)));
				m_startDate = string.IsNullOrEmpty(startdate) ? DateTime.MinValue : DateTime.Parse(startdate);
				m_endDate = string.IsNullOrEmpty(enddate) ? DateTime.MaxValue : DateTime.Parse(enddate);
			}

			public bool Filter(T rsc)
			{
				if(rsc is ILocationalResource locationalResource)
				{
					return locationalResource.Location.Coordinate.GetDistanceTo(m_coordinate) < distance;
				}
				if(rsc is ITimeBoundResource timeboundResource)
				{
					return timeboundResource.StartDate >= m_startDate && timeboundResource.EndDate <= m_endDate;
				}
				else if(!string.IsNullOrEmpty(latlong) || distance.HasValue)
				{
					throw new Exception("Invalid filter. You cannot filter this resource by location");
				}
				return false;
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index(ResourceFilterModel<T> filter = null)
		{
			if(filter != null)
			{
				return Ok(ResourceManager.Get(rsc => filter.Filter(rsc)));
			}
			return Ok(ResourceManager.Get(x => true).Select(rsc => rsc.GUID));
		}
	}
}
