using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using Dodo.Models;

namespace Resources
{
	public abstract class CrudResourceAPIController<T, TSchema> : CustomController
		where T : class, IDodoResource
		where TSchema : ResourceSchemaBase
	{
		protected CrudResourceServiceBase<T, TSchema> PublicService =>
			new CrudResourceServiceBase<T, TSchema>(Context, HttpContext, AuthService);
		protected abstract AuthorizationService<T, TSchema> AuthService { get; }

		[HttpPatch("{id}")]
		public virtual async Task<IActionResult> Update(string id, [FromBody]Dictionary<string, JsonElement> rawValues)
		{
			// This function will just flatten out the nested objects we can be sent
			Dictionary<string, object> Flatten(Dictionary<string, JsonElement> jsonDict)
			{
				var result = new Dictionary<string, object>();
				foreach (var sub in jsonDict)
				{
					if (sub.Value.ValueKind == JsonValueKind.Object)
					{
						result[sub.Key] =
							Flatten(System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(sub.Value.GetRawText()));
					}
					else
					{
						switch (sub.Value.ValueKind)
						{
							case JsonValueKind.String:
								result[sub.Key] = sub.Value.GetString();
								break;
							case JsonValueKind.Number:
								result[sub.Key] = sub.Value.GetDouble();
								break;
							case JsonValueKind.Array:
								result[sub.Key] = sub.Value.EnumerateArray().ToList();
								break;
							case JsonValueKind.Null:
								result[sub.Key] = null;
								break;
							case JsonValueKind.True:
								result[sub.Key] = true;
								break;
							case JsonValueKind.False:
								result[sub.Key] = false;
								break;
							default:
								throw new Exception($"Unsupported JsonValueKind {sub.Value.ValueKind}");
						}
					}
				}
				return result;
			}
			return (await PublicService.Update(id, Flatten(rawValues))).ActionResult;
		}

		[HttpDelete("{id}")]
		public virtual async Task<IActionResult> Delete(string id)
		{
			return (await PublicService.Delete(id)).ActionResult;
		}

		[HttpGet("{id}")]
		public virtual async Task<IActionResult> Get(string id)
		{
			var result = await PublicService.Get(id);
			return result.ActionResult;
		}

		[HttpGet("notifications/{id}")]
		public virtual async Task<IActionResult> GetNotifications([FromRoute]string id, [FromQuery]int page = 1)
		{
			return (await PublicService.GetNotifications(id, page)).ActionResult;
		}

		[HttpPost("notifications/{id}/new")]
		public virtual async Task<IActionResult> PostNotification([FromRoute]string id, [FromBody]NotificationModel notification)
		{
			return (await PublicService.AddNotification(id, notification)).ActionResult;
		}

		[HttpPost("notifications/{id}/delete")]
		public virtual async Task<IActionResult> DeleteNotification([FromRoute]string id, [FromQuery]Guid notification)
		{
			return (await PublicService.DeleteNotification(id, notification)).ActionResult;
		}
	}
}
