using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dodo.ViewModels
{
	/// <summary>
	/// Base DTO to PATCH containing only writable properties
	/// </summary>
	public abstract class ResourceDTOBase
	{
		[DisplayName("ID")]
		public Guid Guid { get; set; }
		[Required]
		[MaxLength(64)]
		public string Name { get; set; }
		[Required]
		[MaxLength(2048)]
		[DisplayName("Public Description")]
		public string PublicDescription { get; set; }
	}
}
