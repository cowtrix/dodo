using Ryadel.Components.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Common.Extensions
{
	public interface IVerifiableMember
	{
		bool Verify(object objectToVerify, out string error);
	}

	/// <summary>
	/// Indicates that a type is able to verify and validate its state
	/// </summary>
	public interface IVerifiable
	{
		bool CanVerify();
		bool VerifyExplicit(out string error);
	}

	public class MemberVerificationException : Exception
	{
		public MemberVerificationException(string message) : base(message)
		{
		}

		public MemberVerificationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	public static class VerificationExtensions
	{
		/// <summary>
		/// This extension method will go through fields and properties marked with verify attributes
		/// The type of verify attribute tells it how to check the value
		/// </summary>
		/// <param name="objectToVerify"></param>
		public static bool Verify(this IVerifiable objectToVerify, out string error)
		{
			if(objectToVerify == null)
			{
				error = null;
				return true;
			}
			var members = objectToVerify.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach(var m in members)
			{
				var memberName = m.Name;
				var attributes = m.GetCustomAttributes(true).OfType<IVerifiableMember>();
				foreach (var attr in attributes)
				{
					if (attr == null)
					{
						continue;
					}
					var value = m.GetValue(objectToVerify);
					if (!attr.Verify(value, out error))
					{
						return false;
					}
				}
			}
			return objectToVerify.VerifyExplicit(out error);
		}
	}

	public static class ValidationExtensions
	{
		const int NAME_MIN_LENGTH = 3;
		const int NAME_MAX_LENGTH = 64;
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

		static IList<string> m_reservedWords = new List<string>()
		{
			//"COORDINATOR", "ADMIN",
		};
		public static bool NameIsValid(string name, out string error)
		{
			if (string.IsNullOrEmpty(name) || name.Length < NAME_MIN_LENGTH || name.Length > NAME_MAX_LENGTH)
			{
				error = $"Name length must be between {NAME_MIN_LENGTH} and {NAME_MAX_LENGTH} characters long";
				return false;
			}
			var reserved = m_reservedWords.FirstOrDefault(w => name.ToUpperInvariant().Contains(w));
			if(!string.IsNullOrEmpty(reserved))
			{
				error = $"Name contains reserved word: " + reserved;
				return false;
			}
			error = null;
			return true;
		}

		public static bool SlugIsValid(string slug, out string error)
		{
			if (string.IsNullOrEmpty(slug) || slug.Length < NAME_MIN_LENGTH || slug.Length > NAME_MAX_LENGTH)
			{
				error = $"Length must be between {NAME_MIN_LENGTH} and {NAME_MAX_LENGTH} characters long";
				return false;
			}
			var rgx = "^[a-z0-9_]*$";
			if(!Regex.IsMatch(slug, rgx))
			{
				error = "Username can only contain alphanumeric characters and _";
				return false;
			}
			error = null;
			return true;
		}

		public static string GenerateStrongPassword()
		{
			string result = "";
			do
			{
				result = PasswordGenerator.Generate();
			}
			while (!IsStrongPassword(result, out _));
			return result;
		}

		public static bool IsStrongPassword(string input, out string error)
		{
			error = string.Empty;

			if (string.IsNullOrWhiteSpace(input))
			{
				error = "Password should not be empty";
				return false;
			}

			const int minCharCount = 8;
			const int maxCharCount = 64;

			//var hasNumber = new Regex(@"[0-9]+");
			//var hasUpperChar = new Regex(@"[A-Z]+");
			//var hasLowerChar = new Regex(@"[a-z]+");
			var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,\-£]");

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

		public static string StripStringForSlug(string input)
		{
			if(string.IsNullOrEmpty(input))
			{
				return input;
			}
			if(input.Length > NAME_MAX_LENGTH)
			{
				input = input.Substring(0, NAME_MAX_LENGTH);
			}
			return Regex.Replace(input.ToLowerInvariant(), "[^a-z0-9_]", "");
		}
	}
}
