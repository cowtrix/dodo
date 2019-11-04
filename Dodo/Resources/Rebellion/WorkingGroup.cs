using SimpleHttpServer.REST;

namespace Dodo.Rebellions
{
	public class WorkingGroup : RebellionResource
	{
		public WorkingGroup(Rebellion owner, string name) : base(owner)
		{
			Name = name;
		}
		[View]
		public string Name { get; private set; }
		public override string ResourceURL => $"{Rebellion.ResourceURL}/wg/{Name}".StripForURL();
	}
}
