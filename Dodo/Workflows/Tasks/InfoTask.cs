﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	public class InfoTask : WorkflowTask
	{
		delegate bool InfoTaskAction(User user, UserMessage message, out ServerMessage response);

		private struct InfoTaskDefinition
		{
			public string HelpString;
			public InfoTaskAction Task;

			public InfoTaskDefinition(string helpString, InfoTaskAction task) : this()
			{
				HelpString = helpString;
				Task = task;
			}
		}

		public static string CommandKey { get { return "INFO"; } }
		public static string HelpString { get { return $"{CommandKey} - update your information with us."; } }

		[JsonIgnore]
		private Dictionary<string, InfoTaskDefinition> m_commands;

		public InfoTask(Workflow workflow) : base(workflow)
		{
			m_commands = new Dictionary<string, InfoTaskDefinition>()
			{
				{ "ARRIVAL", new InfoTaskDefinition("the day you'll arrive to the Autumn Rebellion in London.", UpdateArrival) },
				{ "DEPARTURE", new InfoTaskDefinition("the day you'll leave to the Autumn Rebellion in London.", UpdateDeparture) },
				{ "EMAIL", new InfoTaskDefinition("add or update an email we can contact you at.", UpdateEmail) }
			};
		}

		private bool UpdateEmail(User user, UserMessage message, out ServerMessage response)
		{
			if (message.ContentUpper.FirstOrDefault() == "EMAIL")
			{
				if(!string.IsNullOrEmpty(user.Email))
				{
					response = new ServerMessage($"I've currently got your email as {user.Email}. What would you like to update it to? Please be aware that by sharing this with us, you consent to XR contacting you via this email address.");
				}
				else
				{
					response = new ServerMessage("Okay, what's your email? Please be aware that by sharing this with us, you consent to XR contacting you via this email address.");
				}
				return true;
			}
			if(!ValidationExtensions.EmailIsValid(message.Content.Trim()))
			{
				response = new ServerMessage("Sorry, that didn't seem like a valid email address to me. You can try again, or if you don't want to update your email, reply CANCEL");
				return true;
			}
			user.Email = message.Content.Trim();
			response = new ServerMessage($"Alright, I've updated your email to {user.Email}. " + GetTaskString());
			CurrentCommand = null;
			return true;
		}

		private bool UpdateArrival(User user, UserMessage message, out ServerMessage response)
		{
			if(message.ContentUpper.FirstOrDefault() == "ARRIVAL")
			{
				if(user.StartDate != default)
				{
					response = new ServerMessage($"I've currently got your arrival date as {user.StartDate.ToShortDateString()}. " +
						"Tell me what date will you be arriving to the rebellion. " +
						"For instance, if you were arriving on the 7th of October, you would reply 7/10");
				}
				else
				{
					response = new ServerMessage("Tell me what date will you be arriving to the rebellion. " +
						"For instance, if you were arriving on the 7th of October, you would reply 7/10");
				}

				return true;
			}
			var split = message.ContentUpper.FirstOrDefault().Split('/');
			DateTime startDate;
			try
			{
				if (split.Length != 2 || !int.TryParse(split[0], out var day) || !int.TryParse(split[1], out var month))
				{
					throw new Exception();
				}
				startDate = new DateTime(2019, month, day);
			}
			catch
			{
				response = new ServerMessage($"Sorry, I didn't quite understand that date. For instance, if you were arriving on the 12th of October, you would reply 12/10." +
					" Or if you want to stop updating your arrival date, reply CANCEL.");
				user.Karma--;
				return true;
			}
			if (startDate < new DateTime(2019, 10, 7))
			{
				response = new ServerMessage($"Looks like you're an early bird! But the rebellion starts on the 7th of October, so that's the earliest you can arrive. If that's the case, reply 7/10." +
					" Or if you want to stop updating your arrival date, reply CANCEL.");
				return true;
			}
			user.StartDate = startDate;
			if(user.StartDate > user.EndDate)
			{
				user.EndDate = default;
			}
			response = new ServerMessage($"Okay, I've updated your arrival date! I'll see you on {user.StartDate.ToShortDateString()}. {GetTaskString()}");
			CurrentCommand = null;
			return true;
		}

		private bool UpdateDeparture(User user, UserMessage message, out ServerMessage response)
		{
			if (message.ContentUpper.FirstOrDefault() == "DEPARTURE")
			{
				if (user.StartDate != default)
				{
					response = new ServerMessage($"I've currently got your departure date as {user.EndDate.ToShortDateString()}. " +
						"Tell me what date will you be leaving the rebellion. " +
						"For instance, if you were arriving on the 9th of October, you would reply 9/10");
				}
				else
				{
					response = new ServerMessage("Tell me what date will you be leaving the rebellion. " +
						"For instance, if you were leaving on the 9th of October, you would reply 9/10");
				}
				return true;
			}
			var split = message.ContentUpper.FirstOrDefault().Split('/');
			DateTime endDate;
			try
			{
				if (split.Length != 2 || !int.TryParse(split[0], out var day) || !int.TryParse(split[1], out var month))
				{
					throw new Exception();
				}
				endDate = new DateTime(2019, month, day);
			}
			catch
			{
				response = new ServerMessage($"Sorry, I didn't quite understand that date. For instance, if you were departing on the 12th of October, you would reply 12/10." +
					" Or if you want to stop updating your departure date, reply CANCEL.");
				user.Karma--;
				return true;
			}
			if (endDate < new DateTime(2019, 10, 7))
			{
				response = new ServerMessage($"Looks like you're an early bird! But the rebellion starts on the 7th of October, so that's the earliest you can leave. If that's the case, reply 7/10." +
					" Or if you want to stop updating your arrival date, reply CANCEL.");
				return true;
			}
			if (user.StartDate != default && endDate < user.StartDate)
			{
				response = new ServerMessage($"Hmm, it looks like you're leaving earlier than you said you'd arrive ({user.StartDate}). Do you need to update your arrival date first?" +
					" Or if you want to stop updating your arrival date, reply CANCEL.");
				return true;
			}
			user.EndDate = endDate;
			response = new ServerMessage($"Okay, I've updated your departure date! We'll see you go on {user.EndDate.ToShortDateString()}. {GetTaskString()}");
			CurrentCommand = null;
			return true;
		}

		public string CurrentCommand;

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var user = session.GetUser();
			if(message.ContentUpper.FirstOrDefault() == "CANCEL")
			{
				CurrentCommand = null;
				response = new ServerMessage("Okay, no worries. " + GetTaskString());
				return true;
			}

			// Process existing task
			if (CurrentCommand != null && m_commands[CurrentCommand].Task(user, message, out response))
			{
				return true;
			}

			// Or try and make new one
			if (m_commands.TryGetValue(message.ContentUpper.FirstOrDefault(), out var newTask))
			{
				CurrentCommand = message.ContentUpper.FirstOrDefault();
				return newTask.Task(user, message, out response);
			}

			if(message.ContentUpper.FirstOrDefault() == CommandKey)
			{
				response = new ServerMessage(GetTaskString());
			}
			else
			{
				response = new ServerMessage("Sorry, I didn't understand that. " + GetTaskString());
			}
			return true;
		}

		private string GetTaskString()
		{
			var sb = new StringBuilder("Please tell me what information you want to update:\n");
			sb.AppendLine();
			foreach (var task in m_commands)
			{
				sb.AppendLine($"{task.Key} - {task.Value.HelpString}");
			}
			sb.AppendLine();
			sb.AppendLine("Or, if you're done, reply CANCEL");
			return sb.ToString();
		}

		public override bool CanCancel()
		{
			return string.IsNullOrEmpty(CurrentCommand);
		}
	}
}
