﻿using Common;
using Dodo.Rebellions;
using Dodo.Resources;
using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.WorkingGroups
{
	public class WorkingGroupManager : DodoResourceManager<WorkingGroup>
	{
		public override string BackupPath => "workinggroups";
	}
}