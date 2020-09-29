using Resources.Security;
using System;

namespace Dodo.RoleApplications
{
	public class Message
	{
		public string Sender { get; set; }
		public DateTime Timestamp { get; set; }
		public string Content { get; set; }

		public Message() { }

		public Message(AccessContext context, string content, bool noSender)
		{
			if (!noSender)
			{
				Sender = SecurityExtensions.GenerateID(context.User, context.Passphrase, context.User.Guid.ToString());
			}
			Content = content;
			Timestamp = DateTime.UtcNow;
		}
	}
}
