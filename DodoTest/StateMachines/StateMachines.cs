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
		[TestMethod]
		public void CanLoadStateMachine()
		{
			var firstState = new State("First State");
			var secondState = new State("Second state");
			firstState.AddTransition(secondState, new TransitionAlways());
			var definition = new StateMachineDefinition("Test SM")
				.AddState(firstState)
				.AddState(secondState);

			var json = JsonConvert.SerializeObject(definition, Formatting.Indented, new JsonSerializerSettings()
			{
				PreserveReferencesHandling = PreserveReferencesHandling.All,
				TypeNameHandling = TypeNameHandling.Auto,
			});

			var stateMachine = new StateMachine(json);
		}
	}
}
