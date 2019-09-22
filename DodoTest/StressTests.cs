using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DodoTest;
using XR.Dodo;
using System.Threading;
using System.Diagnostics;
using System;
using System.Collections.Generic;

[TestClass]
public class StressTests : TestBase
{
	[TestMethod]
	public void TelegramStress()
	{
		int threadCount = 10;
		var sm = new SemaphoreSlim(1);
		var tasks = new List<Task>();
		for(var i = 0; i < threadCount; ++i)
		{
			var task = new Task(() =>
			{
				var sw = new Stopwatch();
				sw.Start();
				while(sw.Elapsed < TimeSpan.FromMinutes(1))
				{
					DodoServer.TelegramGateway.FakeMessage("test", 9999);
				}
			});
			tasks.Add(task);
			task.Start();
		}
		Task.WaitAll(tasks.ToArray());
	}
}
