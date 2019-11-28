using Common.StateMachines;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;

namespace Dodo.Tasks
{
	public class WorkflowTask : StateMachine<ClientMessage, ServerMessage>, IDodoResource
	{
		public WorkflowTask(string definition) : base(definition)
		{
			GUID = Guid.NewGuid();
		}

		[JsonProperty]
		public Guid GUID { get; private set; }

		public string ResourceURL => $"tasks/{Definition.Name}";

		public ResourceReference<User> Creator => throw new NotImplementedException();

		public bool IsAuthorised(User requestOwner, string passphrase, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			throw new NotImplementedException();
		}

		public void OnDestroy()
		{
		}
	}

	public class WorkflowTaskDefinition : StateMachineDefinition<ClientMessage, ServerMessage>
	{
	}

	public class MessageState : State<ClientMessage, ServerMessage>
	{
		public MessageState(string name) : base(name)
		{
		}

		public override State<ClientMessage, ServerMessage> OnEntry(ClientMessage input, out ServerMessage output)
		{
			throw new NotImplementedException();
		}

		public override State<ClientMessage, ServerMessage> OnReentry(ClientMessage input, out ServerMessage output)
		{
			throw new NotImplementedException();
		}
	}
}
