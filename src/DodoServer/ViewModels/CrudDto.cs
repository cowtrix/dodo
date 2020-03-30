namespace DodoServer.ViewModels
{
	/// <summary>
	/// Base DTO to PATCH containing only writable properties
	/// </summary>
	public class CrudDto
	{
		public string Name { get; set; }
		public string PublicDescription { get; set; }
	}
}
