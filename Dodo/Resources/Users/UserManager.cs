using Dodo.Resources;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.Users
{
	public class UserManager : DodoResourceManager<User>
	{
		public override string BackupPath => "users";
	}
}
