using System;
using System.Collections.Concurrent;
using System.Linq;

namespace SimpleHttpServer.REST
{
	public class ResourceLock : IDisposable
	{
		private static ConcurrentDictionary<Guid, string> m_locks = new ConcurrentDictionary<Guid, string>();
		public IRESTResource Value { get; private set; }

		public static bool IsLocked(Guid resource)
		{
			return m_locks.ContainsKey(resource);
		}

		public static bool IsLocked(IRESTResource resource)
		{
			if(resource == null)
			{
				return false;
			}
			return IsLocked(resource.GUID) || IsLocked(resource.ResourceURL);
		}

		public static bool IsLocked(string resourceURL)
		{
			return m_locks.Values.Any(x => x == resourceURL);
		}

		public Guid Guid { get; private set; }

		public ResourceLock(Guid resource) : this(ResourceUtility.GetResourceByGuid(resource))
		{
		}

		public ResourceLock(string resourceURL) : this(ResourceUtility.GetResourceByURL(resourceURL))
		{
		}

		public ResourceLock(IRESTResource resource)
		{
			if(resource == null)
			{
				return;
			}
			while (m_locks.ContainsKey(resource.GUID))
			{
			}
			Guid = resource.GUID;
			m_locks[Guid] = resource.ResourceURL;
			Value = resource;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}
				m_locks.TryRemove(Guid, out _);
				disposedValue = true;
			}
		}

		 ~ResourceLock() {
		   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		   Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

	}
}
