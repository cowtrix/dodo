using Common;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Resources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dodo.Analytics
{
	public class AnalyticPool
	{
		[BsonElement("t")]
		public uint UnauthenticatedViewCount { get; set; }
		[BsonElement("a")]
		public uint AuthenticatedViewCount { get; set; }
	}

	public class AnalyticInfo
	{
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
		public Dictionary<long, AnalyticPool> Pool { get; set; } = new Dictionary<long, AnalyticPool>();
	}

	public static class Analytics
	{
		private static PersistentStore<Guid, AnalyticInfo> m_views
			= new PersistentStore<Guid, AnalyticInfo>("analytics", "views");

		private static ConcurrentQueue<Tuple<Guid, bool, long>> m_pending = new ConcurrentQueue<Tuple<Guid, bool, long>>();

		static Analytics()
		{
			Task.Factory.StartNew(async () => await ProcessQueue());
		}

		static async Task ProcessQueue()
		{
			while(true)
			{
				try
				{
					if (m_pending.Any())
					{
						var pending = new Dictionary<Guid, Dictionary<Tuple<bool, long>, int>>();
						while (m_pending.TryDequeue(out var item))
						{
							if(!pending.TryGetValue(item.Item1, out var c))
							{
								c = new Dictionary<Tuple<bool, long>, int>();
								pending[item.Item1] = c;
							}
							var timeKey = new Tuple<bool, long>(item.Item2, item.Item3);
							c.TryGetValue(timeKey, out var viewCount);
							c[timeKey] = viewCount + 1;
							pending[item.Item1] = c;
						}
						foreach(var p in pending)
						{
							if(!m_views.TryGetValue(p.Key, out var vData))
							{
								vData = new AnalyticInfo();
							}
							foreach(var e in p.Value)
							{
								if (!vData.Pool.TryGetValue(e.Key.Item2, out var pool))
								{
									pool = new AnalyticPool();
									vData.Pool[e.Key.Item2] = pool;
								}
								if(e.Key.Item1)
								{
									pool.AuthenticatedViewCount++;
								}
								else
								{
									pool.UnauthenticatedViewCount++;
								}
							}
							
							m_views[p.Key] = vData;
						}
					}
					await Task.Delay(TimeSpan.FromSeconds(1));
				}
				catch(Exception e)
				{
					Logger.Exception(e);
				}
			}
		}

		public static void RegisterView(AccessContext context, IRESTResource rsc, DateTime? dateOverride = null)
		{
#if !DEBUG
			if(dateOverride != null)
			{
				throw new Exception($"Can't override data in production.");
			}
			dateOverride = DateTime.UtcNow;
#endif
			var dt = RoundDateTime(dateOverride ?? DateTime.UtcNow).ToFileTimeUtc();
			m_pending.Enqueue(new Tuple<Guid, bool, long>(rsc.Guid, context.User != null, dt));			
		}

		public static DateTime RoundDateTime(DateTime dt)
		{
			return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
		}

		public static AnalyticInfo GetInfo(Guid guid)
		{
			if (!m_views.TryGetValue(guid, out var info))
			{
				info = new AnalyticInfo();
				m_views[guid] = info;
			}
			return info;
		}

		public static void Clear()
		{
			m_views.Clear();
		}
	}
}
