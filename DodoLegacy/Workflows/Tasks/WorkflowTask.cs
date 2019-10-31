using Newtonsoft.Json;
using System;
using System.Linq;

namespace Dodo.Dodo
{
	[AttributeUsage(AttributeTargets.Class)]
	public class WorkflowTaskInfoAttribute : Attribute
	{
		public readonly EUserAccessLevel MinAccessLevel;
		public WorkflowTaskInfoAttribute(EUserAccessLevel minAccessLevel)
		{
			MinAccessLevel = minAccessLevel;
		}
	}

	public abstract class WorkflowTask
	{
		public DateTime TimeCreated;

		[JsonIgnore]
		public virtual TimeSpan Timeout { get { return TimeSpan.FromMinutes(15); } }

		public abstract bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response);

		public virtual ServerMessage ExitTask(UserSession session)
		{
			session.Workflow.CurrentTask = null;
			return new ServerMessage("Okay, I've canceled that.");
		}

		public virtual bool GetHelp(out ServerMessage response)
		{
			response = default;
			return false;
		}

		public virtual bool CanCancel()
		{
			return true;
		}

		public EUserAccessLevel GetMinimumAccessLevel()
		{
			return GetMinimumAccessLevel(GetType());
		}

		public static EUserAccessLevel GetMinimumAccessLevel(Type t)
		{
			var attr = (WorkflowTaskInfoAttribute)t.GetCustomAttributes(typeof(WorkflowTaskInfoAttribute), true).Single();
			return attr.MinAccessLevel;
		}
	}
}
