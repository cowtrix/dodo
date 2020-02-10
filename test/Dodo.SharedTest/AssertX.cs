using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

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

		public static async Task ThrowsAsync<T>(Task action) where T : Exception
		{
			await ThrowsAsync<T>(action, e => true);
		}

		public static async Task ThrowsAsync<T>(Task action, Func<Exception, bool> exceptionValidator) where T : Exception
		{
			try
			{
				await action;
			}
			catch (T e)
			{
				if (!exceptionValidator(e))
				{
					throw new AssertFailedException("Incorrect exception was thrown: " + e.Message);
				}
				return;
			}
			throw new AssertFailedException("Exception was not thrown");
		}
	}
}
