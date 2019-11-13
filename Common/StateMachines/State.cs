using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Common.StateMachines
{
	public abstract class State<TInput, TOutput>
	{
		public string Name { get; set; }
		public Guid GUID { get; set; }
		public List<Transition<TInput, TOutput>> Transitions { get; set; }

		[JsonIgnore]
		private StateMachine<TInput, TOutput> m_parent;

		public State(string name)
		{
			GUID = Guid.NewGuid();
			Name = name;
			Transitions = new List<Transition<TInput, TOutput>>();
		}

		public State<TInput, TOutput> AddTransition(State<TInput, TOutput> destination, ITransitionCondition<TInput, TOutput> condition)
		{
			Transitions.Add(new Transition<TInput, TOutput>(this, destination, condition));
			return this;
		}

		public abstract State<TInput, TOutput> OnEntry(TInput input, out TOutput output);

		public abstract State<TInput, TOutput> OnReentry(TInput input, out TOutput output);

		public virtual void OnExit(TInput input) { }
	}
}
