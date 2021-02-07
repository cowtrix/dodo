using Resources;
using System.Collections.Generic;

namespace Dodo.Users
{
	public class MyRebellionNode
	{
		public MyRebellionNode(IResourceReference reference)
		{
			Reference = reference;
		}
		public bool Checked { get; set; }
		[View]
		public List<MyRebellionNode> Children { get; set; } = new List<MyRebellionNode>();
		[View]
		public IResourceReference Reference { get; set; }
		[View]
		public bool Administrator { get; set; }
		[View]
		public bool Member { get; set; }
	}
}
