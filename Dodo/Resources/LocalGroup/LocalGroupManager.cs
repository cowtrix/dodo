using Common;
using Dodo.Resources;
using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.LocalGroups
{
	public class LocalGroupManager : DodoResourceManager<LocalGroup>
	{
		public override string BackupPath => "localgroups";
	}
}
