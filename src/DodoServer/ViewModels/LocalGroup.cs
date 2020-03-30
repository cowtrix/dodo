namespace DodoServer.ViewModels
{
	/// <summary>
	/// View Model for local group containing both read-only and writable properties
	/// </summary>
	public class LocalGroup : Group
	{
		public Location Location { get; set; }

	}
}
