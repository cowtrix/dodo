using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.StateMachines
{
	public abstract class State<TInput, TOutput>
	{
		public string Name;
		public List<Transition<TInput, TOutput>> Transitions = new List<Transition<TInput, TOutput>>();

		public State(string name)
		{
			Name = name;
		}

		public abstract TOutput Output(TInput input);

		public State<TInput, TOutput> AddTransition(State<TInput, TOutput> destination, ITransitionCondition<TInput, TOutput> condition)
		{
			Transitions.Add(new Transition<TInput, TOutput>(this, destination, condition));
			return this;
		}

		public abstract State<TInput, TOutput> OnEnter(TInput input);
	}

	public class StateMachineDefinition<TInput, TOutput>
	{
		public string Name;
		public List<State<TInput, TOutput>> States = new List<State<TInput, TOutput>>();
		public State<TInput, TOutput> Root { get; private set; }

		public StateMachineDefinition(string name, State<TInput, TOutput> root)
		{
			Name = name;
			Root = root;
			AddState(root);
		}

		public StateMachineDefinition<TInput, TOutput> AddState(State<TInput, TOutput> state)
		{
			States.Add(state);
			return this;
		}

		public State<TInput, TOutput> Step(State<TInput, TOutput> startingState, TInput input, out TOutput output)
		{
			var nextAction = startingState.Transitions
				.First(transition => transition.Condition.ShouldTransition(startingState, transition.Destination, input));
			output = nextAction.Destination.Output(input);
			return nextAction.Destination.OnEnter(input);
		}
	}

	public class StateMachine<TInput, TOutput, TData>
	{
		public StateMachineDefinition<TInput, TOutput> Definition { get; set; }
		[JsonProperty(IsReference = true, ItemIsReference = true)]
		public State<TInput, TOutput> CurrentState { get; set; }
		public TData DataStore { get; set; }

		public StateMachine(string definition)
		{
			Definition = JsonConvert.DeserializeObject<StateMachineDefinition<TInput, TOutput>>(definition, new JsonSerializerSettings()
			{
				PreserveReferencesHandling = PreserveReferencesHandling.All,
				TypeNameHandling = TypeNameHandling.Auto,
			});
			CurrentState = Definition.Root;
		}

		public TOutput Step(TInput input)
		{
			CurrentState = Definition.Step(CurrentState, input, out var output);
			return output;
		}
	}
}
