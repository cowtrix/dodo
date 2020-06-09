using System;

namespace Resources
{
	public class SearchPriority : Attribute
	{
		public readonly int Priority;

		public SearchPriority(int priority)
		{
			Priority = priority;
		}
	}

}
