using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SharedTest
{
	public static class AssertX
	{
		public static void Throws<T>(Action action, Func<Exception, bool> exceptionValidator) where T:Exception
		{
			try
			{
				action();
			}
			catch(T e)
			{
				if(!exceptionValidator(e))
				{
					throw new AssertFailedException("Incorrect exception was thrown: " + e.Message);
				}
				return;
			}
			throw new AssertFailedException("Exception was not thrown");
		}
	}
}
