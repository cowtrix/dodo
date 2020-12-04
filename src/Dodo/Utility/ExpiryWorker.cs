using Common;
using Dodo;
using Dodo.DodoResources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dodo.Expiry
{
	public static class ExpiryWorker
	{
		public static void Initialise()
		{
			new Task(async () => await Think()).Start();
		}

		private static async Task Think()
		{
			while(true)
			{
				try
				{
					var rscs = DodoResourceUtility.Search(0, int.MaxValue)
						.OfType<ITimeBoundResource>()
						.Where(r => r.EndDate + TimeSpan.FromDays(1) < DateTime.UtcNow);
					foreach(var unpublish in rscs)
					{
						Logger.Info($"Expired resource {unpublish} ({unpublish.EndDate})");
						unpublish.SetPublished(false);
					}
					await Task.Delay(TimeSpan.FromHours(1));
				}
				catch(Exception e)
				{
					Logger.Exception(e, $"Exception in {nameof(ExpiryWorker)}");
				}
			}
		}
	}
}
