using Dodo.Users;
using Resources.Security;
using System.Collections.Generic;

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
	}
}
