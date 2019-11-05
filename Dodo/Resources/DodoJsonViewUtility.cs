using Dodo.Users;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodo.Resources
{
	/*public static class DodoJsonViewUtility
	{
		/// <summary>
		/// This will generate a JSON object that represents viewable (public facing) properties of this object
		/// An object is marked as viewable with the ViewAttribute
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, object> GenerateJsonView(this DodoResource obj, User user)
		{
			if(!obj.IsAuthorised(user, ))
			{
				throw HTTPException.FORBIDDEN;
			}
			return obj.GenerateJsonView(view);
		}

		/// <summary>
		/// This will generate a JSON object that represents viewable (public facing) properties of this object
		/// An object is marked as viewable with the ViewAttribute
		/// </summary>
		/// <returns></returns>
		public static List<Dictionary<string, object>> GenerateJsonView<T>(this IEnumerable<T> obj, User user) where T:DodoResource
		{
			return obj.Select(x => x.GenerateJsonView(user)).ToList();
		}
	}*/
}
