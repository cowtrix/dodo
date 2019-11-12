using System;
using System.Collections.Generic;
using Common.StateMachines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace StateMachines
{
	[TestClass]
	public class StateMachines
	{
		private StateMachine<string, string, string> GetTestStateMachine()
		{
			var firstState = new StringState("First State", "first");
			var secondState = new StringState("Second state", "second");
			firstState.AddTransition(secondState, new TransitionAlways<string, string>());
			var definition = new StateMachineDefinition<string, string>("Test SM", firstState)
				.AddState(secondState);
			var json = JsonConvert.SerializeObject(definition, Formatting.Indented, new JsonSerializerSettings()
			{
				PreserveReferencesHandling = PreserveReferencesHandling.All,
				TypeNameHandling = TypeNameHandling.Auto,
			});
			return new StateMachine<string, string, string>(json);
		}

		[TestMethod]
		public void CanLoad()
		{
			GetTestStateMachine();
		}

		[TestMethod]
		public void CanStep()
		{
			var sm = GetTestStateMachine();
			Assert.AreEqual("second", sm.Step("doesn't matter"));
		}
	}
}
