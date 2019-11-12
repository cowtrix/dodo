using Newtonsoft.Json;

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

	public class Transition<TInput, TOutput>
	{
		[JsonProperty(ItemIsReference = true)]
		public State<TInput, TOutput> Origin;

		[JsonProperty(ItemIsReference = true)]
		public State<TInput, TOutput> Destination;

		public ITransitionCondition<TInput, TOutput> Condition;

		public Transition(State<TInput, TOutput> origin, State<TInput, TOutput> destination, ITransitionCondition<TInput, TOutput> condition)
		{
			Origin = origin;
			Destination = destination;
			Condition = condition;
		}
	}
}
