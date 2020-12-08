using Resources;
using System;

namespace Dodo
{
	public interface IParentResource : IDodoResource
	{
		bool CanContain(Type type);
		void AddChild<T>(AccessContext context, T rsc) where T : class, IOwnedResource;
		bool RemoveChild<T>(AccessContext context, T rsc) where T : class, IOwnedResource;
	}
}
