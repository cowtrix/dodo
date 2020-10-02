using Common;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Resources
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
		private static ConcurrentDictionary<Guid, Guid> m_locks = new ConcurrentDictionary<Guid, Guid>();

		/// <summary>
		/// When using a ResourceLock, always get the value from here. This is guaranteed to be the most
		/// up to date version of the object.
		/// </summary>
		public IRESTResource Value { get; private set; }

		public static bool IsLocked(Guid resource, Guid? handle = null)
		{
			if (handle.HasValue)
			{
				return m_locks.TryGetValue(resource, out var handleGuid) && handleGuid != handle;
			}
			return m_locks.ContainsKey(resource);
		}

		public static bool IsLocked(IRESTResource resource, Guid? handle = null)
		{
			if (resource == null)
			{
				return false;
			}
			return IsLocked(resource.Guid, handle);
		}

		public Guid Guid { get; private set; }
		public Guid Handle { get; private set; }

#if DEBUG
		private System.Diagnostics.StackTrace m_stackTrace;
#endif

		public ResourceLock(Guid resource) : this(ResourceUtility.GetResourceByGuid(resource, force:true))
		{
		}

		public ResourceLock(IRESTResource resource)
		{
			if (resource == null)
			{
				return;
			}
			Logger.Debug($"Locking resource {resource}");
			Handle = Guid.NewGuid();
			while (IsLocked(resource.Guid, Handle) || !m_locks.TryAdd(resource.Guid, Handle))
			{
				Thread.Sleep(10);
			}
			Guid = resource.Guid;
			Value = ResourceUtility.GetResourceByGuid(Guid, Handle);
#if DEBUG
			m_stackTrace = new System.Diagnostics.StackTrace();
#endif
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
			m_locks.TryRemove(Guid, out _); // Remove the lock
			disposedValue = true;
		}
	}

	~ResourceLock()
	{
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
