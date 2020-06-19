using System.Collections.Generic;
using System.Text;

namespace Dodo.Models
{
	public class LoginModel
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Redirect { get; set; }
		public bool RememberMe { get; set; }
	}
}
