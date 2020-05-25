using Common;
using Dodo.Rebellions;
using Dodo.Resources;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.Sites
{
	public class SiteManager : DodoResourceManager<Site>, IResourceManager<EventSite>, IResourceManager<MarchSite>, IResourceManager<PermanentSite>, ISearchableResourceManager
	{
		public void Add(PermanentSite newObject) => base.Add(newObject);
		public void Delete(PermanentSite objToDelete) => base.Delete(objToDelete);
		public IEnumerable<PermanentSite> Get(Func<PermanentSite, bool> selector, Guid? handle = null) => base.Get(r => selector(r as PermanentSite), handle).OfType<PermanentSite>();
		public PermanentSite GetFirst(Func<PermanentSite, bool> selector, Guid? handle = null) => base.GetFirst(r => selector(r as PermanentSite), handle) as PermanentSite;
		public PermanentSite GetSingle(Func<PermanentSite, bool> selector, Guid? handle = null) => base.GetSingle(r => selector(r as PermanentSite), handle) as PermanentSite;
		public void Update(PermanentSite objToUpdate, ResourceLock locker) => base.Update(objToUpdate, locker);

		public void Add(EventSite newObject) => base.Add(newObject);
		public void Delete(EventSite objToDelete) => base.Delete(objToDelete);
		public IEnumerable<EventSite> Get(Func<EventSite, bool> selector, Guid? handle = null) => base.Get(r => selector(r as EventSite), handle).OfType<EventSite>();
		public EventSite GetFirst(Func<EventSite, bool> selector, Guid? handle = null) => base.GetFirst(r => selector(r as EventSite), handle) as EventSite;
		public EventSite GetSingle(Func<EventSite, bool> selector, Guid? handle = null) => base.GetSingle(r => selector(r as EventSite), handle) as EventSite;
		public void Update(EventSite objToUpdate, ResourceLock locker) => base.Update(objToUpdate, locker);

		public void Add(MarchSite newObject) => base.Add(newObject);
		public void Delete(MarchSite objToDelete) => base.Delete(objToDelete);
		public IEnumerable<MarchSite> Get(Func<MarchSite, bool> selector, Guid? handle = null) => base.Get(r => selector(r as MarchSite), handle).OfType<MarchSite>();
		public MarchSite GetFirst(Func<MarchSite, bool> selector, Guid? handle = null) => base.GetFirst(r => selector(r as MarchSite), handle) as MarchSite;
		public MarchSite GetSingle(Func<MarchSite, bool> selector, Guid? handle = null) => base.GetSingle(r => selector(r as MarchSite), handle) as MarchSite;
		public void Update(MarchSite objToUpdate, ResourceLock locker) => base.Update(objToUpdate, locker);
	}
}
