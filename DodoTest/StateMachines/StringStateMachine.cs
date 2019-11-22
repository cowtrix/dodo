using System;
using System.Collections.Generic;
using System.IO;
using Common.StateMachines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace StateMachines
{
	[TestClass]
	public class StringStateMachine
	{
		private StateMachine<string, string> GetTestStateMachine()
		{
			var firstState = new StringState("1st", "first");
			var secondState = new StringState("2nd", "second");
			var thirdState = new StringState("3rd", "third");

			var definition = new StateMachineDefinition<string, string>("Test SM", firstState)
				.AddState(secondState)
				.AddState(thirdState);

			firstState.AddTransition(thirdState, new TransitionInputEquals<string, string>("test"));
			firstState.AddTransition(secondState, new TransitionAlways<string, string>());
			thirdState.AddTransition(firstState, new TransitionInputEquals<string, string>("blah"));

			var json = definition.ToJson();
			return new StateMachine<string, string>(json);
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

			Assert.AreEqual("third", sm.Step("test"));		// Move from 1 -> 3
			Assert.AreEqual("third", sm.Step("invalid"));	// Stay in 3
			Assert.AreEqual("first", sm.Step("blah"));		// Move from 3 -> 1
			Assert.AreEqual("second", sm.Step("blah"));		// Move from 1 -> 2
		}
	}
}
