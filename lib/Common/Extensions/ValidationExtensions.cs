using Ryadel.Components.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Common.Extensions
{
	/// <summary>
	/// Indicates that a type is able to verify and validate its state
	/// </summary>
	public interface IVerifiable
	{
		bool CanVerify();
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public abstract class VerifyMemberBase : Attribute
	{
		public abstract bool Verify(object value, out string validationError);
	}

	public class EmailAttribute : VerifyMemberBase
	{
		public override bool Verify(object value, out string validationError)
		{
			validationError = "Invalid email";
			return ValidationExtensions.EmailIsValid(value as string);
		}
	}

	public class VerifyObjectAttribute : VerifyMemberBase
	{
		public override bool Verify(object value, out string validationError)
		{
			var verifiable = value as IVerifiable;
			return verifiable.Verify(out validationError);
		}
	}

	public class PhoneNumberAttribute : VerifyMemberBase
	{
		public override bool Verify(object value, out string validationError)
		{
			var ph = value as string;
			if (!ValidationExtensions.ValidateNumber(ref ph))
			{
				validationError = "Invalid phone number";
				return false;
			}
			validationError = null;
			return true;
		}
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
				var attr = m.GetCustomAttribute<VerifyMemberBase>();
				if(attr == null)
				{
					continue;
				}
				object value = null;
				if(m is FieldInfo)
				{
					value = (m as FieldInfo).GetValue(objectToVerify);
				}
				else if(m is PropertyInfo)
				{
					value = (m as PropertyInfo).GetValue(objectToVerify);
				}
				else
				{
					continue;
				}
				if(!attr.Verify(value, out error))
				{
					return false;
				}
			}
			error = null;
			return true;
		}
	}

	public class UsernameAttribute : VerifyMemberBase
	{
		public override bool Verify(object value, out string validationError)
		{
			var str = value as string;
			if (!ValidationExtensions.UsernameIsValid(str, out validationError))
			{
				return false;
			}
			return true;
		}
	}

	public class UserFriendlyNameAttribute : VerifyMemberBase
	{
		public override bool Verify(object value, out string validationError)
		{
			var str = value as string;
			if (!ValidationExtensions.NameIsValid(str, out validationError))
			{
				return false;
			}
			return true;
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
				// TODO
				//PhoneNumbers.PhoneNumberUtil.GetInstance().Parse(number, null);
			}
			catch
			{
				return false;
			}
			return true;
			//return Regex.IsMatch(number,
			//	@"^(?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4})?$");
		}

		static IList<string> m_reservedWords = new List<string>()
		{
			"COORDINATOR", "ADMIN",
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

		public static bool UsernameIsValid(string username, out string error)
		{
			if (string.IsNullOrEmpty(username) || username.Length < NAME_MIN_LENGTH || username.Length > NAME_MAX_LENGTH)
			{
				error = $"Name length must be between {NAME_MIN_LENGTH} and {NAME_MAX_LENGTH} characters long";
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