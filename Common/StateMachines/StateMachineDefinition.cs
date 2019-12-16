using Common.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.StateMachines
{
	public class StateMachineDefinition<TInput, TOutput>
	{
		public string Name { get; set; }
		[JsonProperty]
		public List<State<TInput, TOutput>> States { get; private set; }
		[JsonProperty]
		public Guid RootState { get; private set; }

		public StateMachineDefinition() { }

		public StateMachineDefinition(string name, State<TInput, TOutput> root)
		{
			Name = name;
			RootState = root.GUID;
			States = new List<State<TInput, TOutput>>();
			AddState(root);
		}

		public State<TInput, TOutput> GetState(Guid guid)
		{
			return States.SingleOrDefault(x => x.GUID == guid);
		}

		public StateMachineDefinition<TInput, TOutput> AddState(State<TInput, TOutput> state)
		{
			States.Add(state);
			return this;
		}

		public State<TInput, TOutput> Step(State<TInput, TOutput> startingState, TInput input, out TOutput output)
		{
			var nextAction = startingState.Transitions
				.FirstOrDefault(transition => transition.Condition.ShouldTransition(startingState, GetState(transition.DestinationState), input));
			if(nextAction == null)
			{
				return startingState.OnReentry(input, out output);
			}
			return GetState(nextAction.DestinationState).OnEntry(input, out output);
		}

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented, JsonExtensions.DatabaseSettings);
		}
	}
}
