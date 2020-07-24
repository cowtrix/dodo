using Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dodo.Analytics
{
	public class AnalyticInfo
	{
		public uint TotalViewCount { get; set; }
		public uint AuthenticatedViewCount { get; set; }
	}

	public static class Analytics
	{
		private static PersistentStore<Guid, AnalyticInfo> m_views 
			= new PersistentStore<Guid, AnalyticInfo>("analytics", "views");
		public static void RegisterView(AccessContext context, IRESTResource rsc)
		{
			if(!m_views.TryGetValue(rsc.Guid, out var info))
			{
				info = new AnalyticInfo();
			}
			info.TotalViewCount++;
			if(context.User != null)
			{
				info.AuthenticatedViewCount++;
			}
			m_views[rsc.Guid] = info;
		}
	}
}
