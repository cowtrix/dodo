using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace SharedTest
{
	public static class AssertX
	{
		public static void Throws<T>(Action action, Func<Exception, bool> exceptionValidator) where T:Exception
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}
			if (exceptionValidator == null)
			{
				throw new ArgumentNullException(nameof(exceptionValidator));
			}
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
			await ThrowsAsync<T>(action, e => true).ConfigureAwait(true);
		}

		public static async Task ThrowsAsync<T>(Task action, Func<Exception, bool> exceptionValidator) where T : Exception
		{
			if(action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}
			if(exceptionValidator == null)
			{
				throw new ArgumentNullException(nameof(exceptionValidator));
			}
			try
			{
				await action.ConfigureAwait(true);
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

		public static void AreEqual<T>(T first, T second, Func<T, T, bool> equalityOperator = null)
		{
			if(equalityOperator == null)
			{
				equalityOperator = (f, s) => f.Equals(s);
			}
			Assert.IsTrue(equalityOperator(first, second), $"Equality failed. Expected: {first}\nActual: {second}");
		}

		public static void AreNotEqual<T>(T first, T second, Func<T, T, bool> equalityOperator = null)
		{
			if (equalityOperator == null)
			{
				equalityOperator = (f, s) => f.Equals(s);
			}
			Assert.IsFalse(equalityOperator(first, second), $"Equality failed. Expected: {first}\nActual: {second}");
		}
	}
}
