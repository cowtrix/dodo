namespace Dodo.ViewModels
{
	/// <summary>
	/// DTO to PATCH a local group containing only writable properties
	/// </summary>
	public class LocalGroupDto : CrudDto
	{
		public Location Location { get; set; }
	}
}
