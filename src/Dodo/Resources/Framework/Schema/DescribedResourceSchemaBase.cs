using Resources;
using System.ComponentModel;

namespace Dodo
{
	public abstract class DescribedResourceSchemaBase : ResourceSchemaBase
	{
		[View(EPermissionLevel.USER, customDrawer: "markdown")]
		[Resources.MaxStringLength]
		[DisplayName("Public Description")]
		public string PublicDescription { get; set; }

		public DescribedResourceSchemaBase(string name, string publicDescription)
			: base(name)
		{
			PublicDescription = publicDescription;
		}

		public DescribedResourceSchemaBase() : base() { }
	}
}
