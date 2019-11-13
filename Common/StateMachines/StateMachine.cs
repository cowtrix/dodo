using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.StateMachines
{
	public class StateMachine<TInput, TOutput>
	{
		[JsonProperty(IsReference = true, ItemIsReference = true)]
		public StateMachineDefinition<TInput, TOutput> Definition { get; set; }
		public Guid CurrentState { get; set; }
		public Dictionary<string, object> DataStore = new Dictionary<string, object>();

		public StateMachine(string definition)
		{
			Definition = JsonConvert.DeserializeObject<StateMachineDefinition<TInput, TOutput>>(definition, new JsonSerializerSettings()
			{
				//PreserveReferencesHandling = PreserveReferencesHandling.All,
				TypeNameHandling = TypeNameHandling.Auto,
			});
			CurrentState = Definition.RootState;
		}

		public TOutput Step(TInput input)
		{
			CurrentState = Definition.Step(Definition.GetState(CurrentState), input, out var output).GUID;
			return output;
		}
	}
}
