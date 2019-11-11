using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.StateMachines
{
	public class State
	{
		public string Name;
		public List<Transition> Transitions = new List<Transition>();

		public State(string name)
		{
			Name = name;
		}

		public State AddTransition(State destination, ITransitionCondition condition)
		{
			Transitions.Add(new Transition(this, destination, condition));
			return this;
		}
	}

	public class StateMachineDefinition
	{
		public string Name;
		public List<State> States = new List<State>();

		public StateMachineDefinition(string name)
		{
			Name = name;
			States = new List<State>()
			{
				new State("Root")
			};
		}

		public StateMachineDefinition AddState(State state)
		{
			States.Add(state);
			return this;
		}
	}

	public class StateMachine
	{
		internal StateMachineDefinition Definition;
		internal State CurrentState;

		public StateMachine(string definition)
		{
			Definition = JsonConvert.DeserializeObject<StateMachineDefinition>(definition, new JsonSerializerSettings()
			{
				PreserveReferencesHandling = PreserveReferencesHandling.All,
				TypeNameHandling = TypeNameHandling.Auto,
			});
		}
	}
}
