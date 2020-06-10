using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Resources;
using Dodo.ViewModels;
using Dodo.Rebellions;

namespace Dodo.UnitTests.Views
{
	public class ViewModelTestBase<T, TV>
		where T : new()
		where TV : new()
	{
		[TestMethod]
		public void ConvertToViewModel(T rsc)
		{
			var view = rsc.CopyByValue<TV>();
		}
	}


	[TestClass]
	public class RebellionViewModelTests : ViewModelTestBase<Rebellion, RebellionViewModel>
	{
	}
}
