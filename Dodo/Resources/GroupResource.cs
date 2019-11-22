using System;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer.REST;

namespace Dodo
{
	/// <summary>
	/// A group resource is either a Rebellion, Working Group or a Local Group
	/// </summary>
	public abstract class GroupResource : DodoResource
	{
		[JsonProperty]
		public ResourceReference<GroupResource> Parent { get; private set; }

		/// <summary>
		/// Is this object a child of the target object
		/// </summary>
		/// <param name="targetObject"></param>
		/// <returns></returns>
		public bool IsChildOf(GroupResource targetObject)
		{
			if(Parent.Value == null)
			{
				return false;
			}
			if(Parent.Value == targetObject)
			{
				return true;
			}
			return Parent.Value.IsChildOf(targetObject);
		}

		public GroupResource(User creator, GroupResource parent) : base(creator)
		{
			Parent = new ResourceReference<GroupResource>(parent);
		}

		public abstract bool CanContain(Type type);
	}
}
