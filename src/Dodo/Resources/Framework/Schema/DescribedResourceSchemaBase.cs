using Common;
using Resources;
using System.ComponentModel;

namespace Dodo
{
	public abstract class DescribedResourceSchemaBase : ResourceSchemaBase
	{
		[View(EPermissionLevel.USER, customDrawer: "markdown", inputHint:IDescribedResource.MARKDOWN_INPUT_HINT)]
		[Resources.MaxStringLength]
		[Name("Public Description")]
		public string PublicDescription { get; set; }

		public DescribedResourceSchemaBase(string name, string publicDescription)
			: base(name)
		{
			PublicDescription = publicDescription;
		}

		public DescribedResourceSchemaBase() : base() { }
	}
}
