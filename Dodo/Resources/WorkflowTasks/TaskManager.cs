﻿using Common;
using Dodo.Resources;
using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.Tasks
{
	public class WorkflowTaskManager : DodoResourceManager<WorkflowTask>
	{
		public override string BackupPath => "tasks";
	}
}