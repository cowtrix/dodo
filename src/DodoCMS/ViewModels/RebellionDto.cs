using System;

namespace Dodo.ViewModels
{
	/// <summary>
	/// DTO to PATCH a rebellion containing only writable properties
	/// </summary>
	public class RebellionDto : CrudDto
	{
		public Location Location { get; set; }
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
	}
}
