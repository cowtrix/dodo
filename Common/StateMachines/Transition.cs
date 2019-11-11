using Newtonsoft.Json;

namespace Common.StateMachines
{
	public interface ITransitionCondition
	{
		bool ShouldTransition(State origin, State destination, params object[] args);
	}

	[Name("Always")]
	public struct TransitionAlways : ITransitionCondition
	{
		public bool ShouldTransition(State origin, State destination, params object[] args)
		{
			return true;
		}
	}

	public class Transition
	{
		[JsonProperty(IsReference = true, ItemIsReference = true)]
		public State Origin;

		[JsonProperty(IsReference = true, ItemIsReference = true)]
		public State Destination;

		public ITransitionCondition Condition;

		public Transition(State origin, State destination, ITransitionCondition condition)
		{
			Origin = origin;
			Destination = destination;
			Condition = condition;
		}
	}
}
