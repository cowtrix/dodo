using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dodo.ViewModels
{
	/// <summary>
	/// Base View Model for working / local group containing both read-only and writable properties
	/// </summary>
	public class Group
	{
		[DisplayName("Group ID")]
		public Guid Guid { get; set; }

		[Required]
		[MaxLength(64)]
		public string Name { get; set; }

		[Required]
		[MaxLength(2048)]
		[DisplayName("Public Description")]
		public string PublicDescription { get; set; }

		[DisplayName("Member Count")]
		public int MemberCount { get; set; }

	}
}
