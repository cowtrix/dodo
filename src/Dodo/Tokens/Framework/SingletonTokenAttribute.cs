using System;

namespace Dodo.Users.Tokens
{
	public class SingletonTokenDuplicateException : Exception
	{
		public SingletonTokenDuplicateException(string message) : base(message)
		{
		}
	}

	public class SingletonTokenAttribute : Attribute
	{
	}
}
