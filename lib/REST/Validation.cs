using Common.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Resources
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public abstract class CustomValidator : ValidationAttribute, IVerifiableMember
	{
		public abstract bool Verify(object value, out string validationError);

		public override bool IsValid(object value)
		{
			var result = Verify(value, out var error);
			ErrorMessage = error;
			return result;
		}
	}

	public class SlugAttribute : CustomValidator
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

	public class EmailAttribute : CustomValidator
	{
		public override bool Verify(object value, out string validationError)
		{
			validationError = "Invalid email";
			return ValidationExtensions.EmailIsValid(value as string);
		}
	}

	public class VerifyObjectAttribute : CustomValidator
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

	public class UserFriendlyNameAttribute : CustomValidator
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

	public class DescriptionAttribute : CustomValidator
	{
		public const int MAX_DESCRIPTION_LENGTH = 2048;
		public override bool Verify(object value, out string validationError)
		{
			if (value == null)
			{
				validationError = null;
				return true;
			}
			var str = value as string;
			if (str.Length > MAX_DESCRIPTION_LENGTH)
			{
				validationError = $"Text was too long at {str.Length} characters - maximum {MAX_DESCRIPTION_LENGTH} characters";
				return false;
			}
			validationError = null;
			return true;
		}
	}

	public class PasswordAttribute : CustomValidator
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
