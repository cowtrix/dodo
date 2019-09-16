using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace XR.Dodo
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
			return Regex.IsMatch(number,
				@"^(?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4})?$");
		}
	}
}
