using Common;
using REST;
using System.Threading;

namespace Dodo
{
	public static class Dodo
	{
		public const string PRODUCT_NAME = "Dodo";
		private static DodoRESTManager m_restHandler;
		private static CommandReader m_commandReader = new CommandReader();

		static void Main(string[] args)
		{
			Initialise();
			Thread.Sleep(-1);
		}

		public static void Initialise(string[] args = null)
		{
			m_restHandler = new DodoRESTManager();
		}
	}
}
