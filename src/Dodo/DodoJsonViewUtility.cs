using Dodo.Users;
using Resources.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Resources
{
	public static class DodoJsonViewUtility
	{
		
		/// <summary>
		/// This will generate a JSON object that represents viewable properties of this object.
		/// An object is marked as viewable with the ViewAttribute. Fields and properties
		/// are filtered by the requester's EPermissionLevel
		/// </summary>
		/// <returns>A string/object dictionary where the string value is the name of a field and the object is its value</returns>
		public static Dictionary<string, object> GenerateJsonView(this object obj, EPermissionLevel visibility,
			User requester, Passphrase passphrase)
		{
			return JsonViewUtility.GenerateJsonView(obj, visibility, new ResourceReference<User>(requester), passphrase);
		}

		/// <summary>
		/// Generate a JSON view of an IEnumerable.
		/// </summary>
		/// <returns></returns>
		public static List<Dictionary<string, object>> GenerateJsonViewEnumerable<T>(this IEnumerable<T> obj,
			EPermissionLevel visibility, User requester, Passphrase passphrase)
		{
			return obj.Select(x => x.GenerateJsonView(visibility, requester, passphrase)).ToList();
		}
	}
}
