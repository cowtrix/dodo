using SimpleHttpServer.REST;

namespace Dodo.Rebellions
{
	public class LocalGroup : RebellionResource
	{
		public LocalGroup(Rebellion owner, string name) : base(owner)
		{
			Name = name;
		}
		[View]
		public string Name { get; private set; }
		public override string ResourceURL => $"lg/{Name}".StripForURL();
	}
}
