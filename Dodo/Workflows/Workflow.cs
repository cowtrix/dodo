using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XR.Dodo
{
	public class Workflow
	{
		public WorkflowTask CurrentTask;

		[JsonIgnore]
		public Dictionary<string, Type> Tasks = new Dictionary<string, Type>();

		public Workflow()
		{
			AddTask<VerificationTask>(); // TODO Blocker is twilio account :(
			AddTask<HelpTask>();
			AddTask<InfoTask>();
			AddTask<RolesTask>();
			AddTask<MuteTask>();
			AddTask<CoordinatorWhoIsTask>();
			AddTask<CoordinatorNeedsTask>();
			AddTask<CoordinatorRemoveNeedTask>();

			CurrentTask = new IntroductionTask();
		}

		string[] m_greetingStrings = new[]
		 {
			"HELLO", "HI",
		 };


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
			if(!user.Active)
			{
				user.Active = true;
			}

			// First we check if it's a valid role code
			if(DodoServer.CoordinatorNeedsManager.CurrentNeeds.TryGetValue(message.Content.Trim(), out var need) && need.UserIsValidCandidate(user))
			{
				if(need.ConfirmedVolunteers.ContainsKey(user.UUID))
				{
					return new ServerMessage("You've already applied for this Volunteer Request.");
				}
				else
				{
					return need.AddConfirmation(user);
				}
			}

			ServerMessage response;
			if(CurrentTask != null)
			{
				// We have a task underway, so we process that
				if(CurrentTask.CanCancel() && message.ContentUpper.FirstOrDefault() == "DONE")
				{
					// Cancel the request
					return CurrentTask.ExitTask(session);
				}
				if(CurrentTask.GetMinimumAccessLevel() > user.AccessLevel)
				{
					CurrentTask.ExitTask(session);
					return new ServerMessage("Sorry, it doesn't look like you're authorised to do that."
						+ (user.IsVerified() ? "" : " Please verify your account and try again."));
				}
				// Do task specific help
				if (message.ContentUpper.FirstOrDefault() == HelpTask.CommandKey)
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
			else if(message.ContentUpper.FirstOrDefault() == "DONE")
			{
				return new ServerMessage("It doesn't look like you're doing anything right now that I can cancel.");
			}
			else if (Tasks.TryGetValue(message.ContentUpper.FirstOrDefault(), out var newWorkflowType))
			{
				var newWorkflow = Activator.CreateInstance(newWorkflowType) as WorkflowTask;
				if (newWorkflow.GetMinimumAccessLevel() > user.AccessLevel)
				{
					return new ServerMessage("Sorry, it doesn't look like you're authorised to do that."
						+ (user.IsVerified() ? "" : " Please verify your account and try again."));
				}
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
			else if(m_greetingStrings.Contains(message.ContentUpper.FirstOrDefault()))
			{
				return new ServerMessage($"Hello{(!string.IsNullOrEmpty(user.Name) ? " " + user.Name : "")}! If you'd like to see a list of things that you can ask me to do, just reply {HelpTask.CommandKey}");
			}
			return DidntUnderstand(user);
		}

		ServerMessage DidntUnderstand(User user)
		{
			user.Karma--;
			return new ServerMessage($"Sorry, I didn't understand that. If you'd like some help, reply {HelpTask.CommandKey}");
		}
	}
}
