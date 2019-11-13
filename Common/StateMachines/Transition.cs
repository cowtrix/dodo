using Newtonsoft.Json;
using System;

namespace Common.StateMachines
{
	public interface ITransitionCondition<TInput, TOutput>
	{
		bool ShouldTransition(State<TInput, TOutput> origin, State<TInput, TOutput> destination, TInput input);
	}

	[Name("Always")]
	public struct TransitionAlways<TInput, TOutput> : ITransitionCondition<TInput, TOutput>
	{
		public bool ShouldTransition(State<TInput, TOutput> origin, State<TInput, TOutput> destination, TInput input)
		{
			return true;
		}
	}

	[Name("Input Equals")]
	public struct TransitionInputEquals<TInput, TOutput> : ITransitionCondition<TInput, TOutput>
	{
		public TInput Value;
		public TransitionInputEquals(TInput value)
		{
			Value = value;
		}

		public bool ShouldTransition(State<TInput, TOutput> origin, State<TInput, TOutput> destination, TInput input)
		{
			return Value.Equals(input);
		}
	}


	public class Transition<TInput, TOutput>
	{
		public Guid OriginState { get; set; }

		public Guid DestinationState { get; set; }

		public ITransitionCondition<TInput, TOutput> Condition { get; set; }

		public Transition() { }

		public Transition(State<TInput, TOutput> origin, State<TInput, TOutput> destination, ITransitionCondition<TInput, TOutput> condition)
		{
			OriginState = origin.GUID;
			DestinationState = destination.GUID;
			Condition = condition;
		}
	}
}
