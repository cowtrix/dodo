﻿using Newtonsoft.Json;
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
		public Dictionary<string, Type> Tasks = new Dictionary<string, Type>();

		public Workflow()
		{
			//kingif(DodoServer.Dummy)
				AddTask<VerificationTask>(); // TODO Blocker is twilio account :(

			AddTask<HelpTask>();
			AddTask<InfoTask>();

			CurrentTask = new IntroductionTask();
		}

		protected void AddTask<T>() where T: WorkflowTask
		{
			var cmd = typeof(T).GetProperty("CommandKey", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string;
			if(string.IsNullOrEmpty(cmd))
			{
				throw new Exception("Command key was empty for type " + typeof(T));
			}
			Tasks.Add(cmd, typeof(T));
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
					return CurrentTask.ExitTask(session);
				}
				// Do task specific help
				if (message.ContentUpper.FirstOrDefault() == "HELP")
				{
					if(CurrentTask.GetHelp(out var helpResponse))
					{
						return helpResponse;
					}
				}
				if (DateTime.Now - CurrentTask.TimeCreated > CurrentTask.Timeout)
				{
					CurrentTask.ExitTask(session);
				}
				else if(CurrentTask.ProcessMessage(message, session, out response))
				{
					return response;
				}
			}
			else if(message.ContentUpper.FirstOrDefault() == "CANCEL")
			{
				return new ServerMessage("It doesn't look like you're doing anything right now that I can cancel.");
			}
			else if (Tasks.TryGetValue(message.ContentUpper.FirstOrDefault(), out var newWorkflowType))
			{
				var newWorkflow = Activator.CreateInstance(newWorkflowType) as WorkflowTask;
                newWorkflow.TimeCreated = DateTime.Now;
				if(newWorkflow != null)
				{
					CurrentTask = newWorkflow;
				}
				if (CurrentTask.ProcessMessage(message, session, out response))
				{
					return response;
				}
			}
			else if(ProcessMessageForRole(message, session, out response))
			{
				return response;
			}
			return DidntUnderstand(user);
		}

		protected abstract bool ProcessMessageForRole(UserMessage message, UserSession session, out ServerMessage response);

		ServerMessage DidntUnderstand(User user)
		{
			user.Karma--;
			return new ServerMessage("Sorry, I didn't understand that. If you'd like some help, reply HELP");
		}
	}
}
