using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.StateMachines
{
	public class StringState : State<string, string>
	{
		public string OutputString { get; set; }

		public StringState(string name, string output) : base(name)
		{
			OutputString = output;
		}

		public override State<string, string> OnEnter(string input)
		{
			return this;
		}

		public override string Output(string input)
		{
			return OutputString;
		}
	}
}
