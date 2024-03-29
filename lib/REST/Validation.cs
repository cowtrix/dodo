using Common.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Resources
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public abstract class CustomValidatorAttribute : ValidationAttribute, IVerifiableMember
	{
		public abstract bool Verify(object value, out string validationError);

		public override bool IsValid(object value)
		{
			var result = Verify(value, out var error);
			ErrorMessage = error;
			return result;
		}
	}

	public class SlugAttribute : CustomValidatorAttribute
	{
		public override bool Verify(object value, out string validationError)
		{
			var str = value as string;
			if (!ValidationExtensions.SlugIsValid(str, out validationError))
			{
				return false;
			}
			return true;
		}
	}

	public class EmailAttribute : CustomValidatorAttribute
	{
		public override bool Verify(object value, out string validationError)
		{
			validationError = "Invalid email";
			return ValidationExtensions.EmailIsValid(value as string);
		}
	}

	public class VerifyObjectAttribute : CustomValidatorAttribute
	{
		public override bool Verify(object value, out string validationError)
		{
			if (value == null)
			{
				validationError = null;
				return true;
			}
			var verifiable = value as IVerifiable;
			if (verifiable != null && !verifiable.VerifyExplicit(out validationError))
			{
				return false;
			}
			return verifiable.Verify(out validationError);
		}
	}

	public class UserFriendlyNameAttribute : CustomValidatorAttribute
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

	public class MaxStringLengthAttribute : CustomValidatorAttribute
	{
		private readonly int m_maxLength;

		public MaxStringLengthAttribute(int maxLength = 2048)
		{
			m_maxLength = maxLength;
		}

		public override bool Verify(object value, out string validationError)
		{
			if (value == null)
			{
				validationError = null;
				return true;
			}
			var str = value as string;
			if (str.Length > m_maxLength)
			{
				validationError = $"Text was too long at {str.Length} characters - maximum {m_maxLength} characters";
				return false;
			}
			validationError = null;
			return true;
		}
	}

	public class PasswordAttribute : CustomValidatorAttribute
	{
		public override bool Verify(object value, out string validationError)
		{
			var str = value as string;
			if (!ValidationExtensions.IsStrongPassword(str, out validationError))
			{
				return false;
			}
			return true;
		}
	}
}
