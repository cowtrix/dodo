using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace XR.Dodo
{
	public static class Utility
	{
		public static IEnumerable<T> ConcatenateCollection<T>(this IEnumerable<IEnumerable<T>> sequences)
		{
			return sequences.SelectMany(x => x);
		}

		public static string RandomString(int length, string seed)
		{
			var random = new Random(seed.GetHashCode());
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
	

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
			if(number.StartsWith("44"))
			{
				if (number.Length != 12)
				{
					return false;
				}
				number = "+" + number;
			}
			if(!number.StartsWith("44"))
			{
				if(number.StartsWith("07"))
				{
					if(number.Length != 11)
					{
						return false;
					}
					number = "+44" + number.Substring(1);
				}
				else if (number.StartsWith("7"))
				{
					if (number.Length != 10)
					{
						return false;
					}
					number = "+44" + number;
				}
			}
			return Regex.IsMatch(number,
				@"^(?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4})?$");
		}
	}
}
