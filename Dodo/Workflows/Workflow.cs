using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XR.Dodo
{
	public abstract class Workflow
	{
		public WorkflowTask CurrentTask;

		[JsonIgnore]
		private Dictionary<string, Type> m_tasks = new Dictionary<string, Type>();

		public Workflow()
		{
			AddTask<Verification>();
			CurrentTask = new IntroductionTask(this);
		}

		protected void AddTask<T>() where T: WorkflowTask
		{
			var cmd = typeof(T).GetProperty("CommandKey", BindingFlags.Public | BindingFlags.Static).GetValue(null) as string;
			if(string.IsNullOrEmpty(cmd))
			{
				throw new Exception("Command key was empty for type " + typeof(T));
			}
			m_tasks.Add(cmd, typeof(T));
		}

		public ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			session.Inbox.Add(message);
			var user = session.GetUser();
			ServerMessage response;
			if(CurrentTask != null)
			{
				// We have a task underway, so we process that
				if(CurrentTask.CanCancel() && message.ContentUpper.FirstOrDefault() == "CANCEL")
				{
					// Cancel the request
					return CurrentTask.ExitTask();
				}
				if (message.ContentUpper.FirstOrDefault() == "HELP")
				{
					if(CurrentTask.GetHelp(out var helpResponse))
					{
						return helpResponse;
					}
				}
				if (DateTime.Now - CurrentTask.TimeCreated > CurrentTask.Timeout)
				{
					CurrentTask.ExitTask();
				}
				else if(CurrentTask.ProcessMessage(message, session, out response))
				{
					return response;
				}
			}
			if(m_tasks.TryGetValue(message.ContentUpper.FirstOrDefault(), out var newWorkflowType))
			{
				var newWorkflow = Activator.CreateInstance(newWorkflowType, this) as WorkflowTask;
				if(newWorkflow != null)
				{
					CurrentTask = newWorkflow;
				}
				if (CurrentTask.ProcessMessage(message, session, out response))
				{
					return response;
				}
			}
			if(ProcessMessageForRole(message, session, out response))
			{
				return response;
			}
			return DidntUnderstand();
		}

		protected abstract bool ProcessMessageForRole(UserMessage message, UserSession session, out ServerMessage response);

		ServerMessage DidntUnderstand()
		{
			return new ServerMessage("Sorry, I didn't understand that. If you'd like some help, reply HELP");
		}
	}
}
