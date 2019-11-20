using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Common
{

	public static class ValidationExtensions
	{
		static Regex ValidEmailRegex = CreateValidEmailRegex();

		/// <summary>
		/// Taken from http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
		/// </summary>
		/// <returns></returns>
		private static Regex CreateValidEmailRegex()
		{
			string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
				+ @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
				+ @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

			return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
		}

		public static bool EmailIsValid(string emailAddress)
		{
			bool isValid = ValidEmailRegex.IsMatch(emailAddress);
			return isValid;
		}

		public static bool ValidateNumber(ref string number)
		{
			number = number.Replace(" ", "").Replace("-", "");
			if (string.IsNullOrEmpty(number))
			{
				return true;
			}
			if(!number.StartsWith("+44"))
			{
				if(number.StartsWith("07"))
				{
					number = "+44" + number.Substring(1);
				}
				else if (number.StartsWith("7"))
				{
					number = "+44" + number;
				}
				else if (number.StartsWith("44"))
				{
					number = "+" + number;
				}
			}
			if (number.StartsWith("00"))
			{
				// Replace 00 at beginning with +
				number = "+" + number.Remove(0, 2);
			}
			if(!number.StartsWith("+"))
			{
				number = "+" + number;
			}
			try
			{
				PhoneNumbers.PhoneNumberUtil.GetInstance().Parse(number, null);
			}
			catch
			{
				return false;
			}
			return true;
			//return Regex.IsMatch(number,
			//	@"^(?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4})?$");
		}

		public static bool UsernameIsValid(string username, out string error)
		{
			if(username.Length < 3)
			{
				error = "Username must be at least 3 characters";
				return false;
			}
			var rgx = "^[a-zA-Z0-9_]*$";
			if(!Regex.IsMatch(username, rgx))
			{
				error = "Username can only contain alphanumeric characters and _";
				return false;
			}
			if(username != username.StripForURL())
			{
				error = $"Username was invalid, Expected: {username.StripForURL()} Received: {username}";
				return false;
			}
			error = null;
			return true;
		}

		public static bool StrongPassword(string input, out string error)
		{
			error = string.Empty;

			if (string.IsNullOrWhiteSpace(input))
			{
				error = "Password should not be empty";
				return false;
			}

			const int minCharCount = 8;
			const int maxCharCount = 20;

			//var hasNumber = new Regex(@"[0-9]+");
			//var hasUpperChar = new Regex(@"[A-Z]+");
			//var hasLowerChar = new Regex(@"[a-z]+");
			var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

			/*if (!hasLowerChar.IsMatch(input))
			{
				error = "Password should contain At least one lower case letter";
				return false;
			}*/
			/*else if (!hasUpperChar.IsMatch(input))
			{
				error = "Password should contain At least one upper case letter";
				return false;
			}*/
			if (input.Length < minCharCount || input.Length > maxCharCount)
			{
				error = $"Password should be between {minCharCount} and {maxCharCount} characters";
				return false;
			}
			/*else if (!hasNumber.IsMatch(input))
			{
				error = "Password should contain At least one numeric value";
				return false;
			}*/
			else if (!hasSymbols.IsMatch(input))
			{
				error = "Password should contain at least one symbol";
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
