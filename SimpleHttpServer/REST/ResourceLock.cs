using Common;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace SimpleHttpServer.REST
{
	/// <summary>
	/// <para>A ResourceLock will prevent a Resource from being altered while it is being edited. It is
	/// meant to handle concurrent edit requests from clients by blocking requests until the
	/// last lock is released.</para>
	/// <para>When an update request is submitted, or a resource is being altered, a ResourceLock
	/// should be placed upon it. You cannot submit the resource for updating back into the database
	/// without a ResourceLock for that resource.</para>
	/// </summary>
	public class ResourceLock : IDisposable
	{
		private static ConcurrentDictionary<Guid, string> m_locks = new ConcurrentDictionary<Guid, string>();

		/// <summary>
		/// When using a ResourceLock, always get the value from here. This is guaranteed to be the most
		/// up to date version of the object.
		/// </summary>
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
			while (IsLocked(resource.GUID) || !m_locks.TryAdd(resource.GUID, resource.ResourceURL))
			{
			}
			Guid = resource.GUID;
			Value = ResourceUtility.GetResourceByGuid(Guid);
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
				m_locks.TryRemove(Guid, out _);	// Remove the lock
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
		}
		#endregion

	}
}
