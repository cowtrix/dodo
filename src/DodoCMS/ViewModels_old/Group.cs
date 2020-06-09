using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dodo.ViewModels
{
	/// <summary>
	/// Base View Model for working / local group containing both read-only and writable properties
	/// </summary>
	public abstract class GroupResourceDTOBase : ResourceDTOBase
	{
		[DisplayName("Member Count")]
		public int MemberCount { get; set; }
	}
}
