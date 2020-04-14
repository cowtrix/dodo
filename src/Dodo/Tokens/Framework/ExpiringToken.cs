using Newtonsoft.Json;
using System;

namespace Dodo.Users.Tokens
{
	public abstract class ExpiringToken : UserToken, IRemovableToken
	{
		[JsonIgnore]
		public bool ShouldRemove => DateTime.Now > ExpiryDate;
		[JsonProperty]
		public DateTime ExpiryDate { get; protected set; }

		public abstract void OnRemove(User parent);

		public ExpiringToken() { }

		public ExpiringToken(DateTime expiry)
		{
			ExpiryDate = expiry;
		}
	}

}
