namespace Dodo.ViewModels
{
	/// <summary>
	/// DTO to PATCH a local group containing only writable properties
	/// </summary>
	public class LocalGroupDto : GroupResourceDTOBase
	{
		public Location Location { get; set; }
	}
}
