using System;

namespace DodoServer.ViewModels
{
	/// <summary>
	/// DTO to PATCH a rebellion containing only writable properties
	/// </summary>
	public class RebellionDto
	{
		public string Name { get; set; }
		public string PublicDescription { get; set; }
		public Location Location { get; set; }
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
	}
}
