using Common.Extensions;
using Dodo.Users;
using Resources;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Dodo.Roles
{
	public class EmailUsernameListAttribute : CustomValidatorAttribute
	{
		public static bool TryGetEmails(string value, out List<string> emails, out string error)
		{
			var userManager = ResourceUtility.GetManager<User>();
			var split = value.Split(new[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
			emails = new List<string>();
			foreach (var str in split)
			{
				var email = str;
				if (!ValidationExtensions.EmailIsValid(email))
				{
					// try username
					var user = userManager.GetSingle(u => u.Slug == email);
					if (user == null)
					{
						error = $"Couldn't parse value: {str}";
						return false;
					}
					email = user.PersonalData.Email;
				}
				emails.Add(email);
			}
			if(!emails.Any())
			{
				error = "Value cannot be empty";
				return false;
			}
			error = null;
			return true;
		}


		public override bool Verify(object value, out string validationError)
		{
			if(!(value is string str))
			{
				validationError = "Bad Type";
				return false;
			}
			return TryGetEmails(str, out _, out validationError);
		}
	}
}
