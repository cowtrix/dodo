using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dodo.ViewModels
{
	/// <summary>
	/// View Model for rebellion containing both read-only and writable properties
	/// </summary>
	public class Rebellion
	{
		[DisplayName("Rebellion ID")]
		public Guid GUID { get; set; }

		[Required]
		[MaxLength(64)]
		public string Name { get; set; }

		[Required]
		[MaxLength(2048)]
		[DisplayName("Public Description")]
		public string PublicDescription { get; set; }

		[DisplayName("Member Count")]
		public int MemberCount { get; set; }

		public Location Location { get; set; }

		[DisplayName("Start Date")]
		public DateTimeOffset StartDate { get; set; }

		[DisplayName("End Date")]
		public DateTimeOffset EndDate { get; set; }
	}
}
