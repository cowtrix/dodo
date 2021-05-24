using System.Collections.Generic;
using Resources;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Dodo
{
	public class UserStore
	{
		public int Count => m_subscriptions.Count;

		[BsonElement]
		private HashSet<HashedResourceReference> m_subscriptions = new HashSet<HashedResourceReference>();

		public void Subscribe(GroupResource parent, AccessContext context)
		{
			var refhash = new HashedResourceReference(context.User.Guid, parent.Guid.ToString());
			m_subscriptions.Add(refhash);
		}

		public void Unsubscribe(GroupResource parent, AccessContext context)
		{
			var refhash = new HashedResourceReference(context.User.Guid, parent.Guid.ToString());
			m_subscriptions.Remove(refhash);
		}

		public bool IsSubscribed(GroupResource parent, Guid user)
		{
			var refhash = new HashedResourceReference(user, parent.Guid.ToString());
			return m_subscriptions.Contains(refhash);
		}
	}
}
