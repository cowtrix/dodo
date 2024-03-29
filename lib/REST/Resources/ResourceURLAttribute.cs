﻿using Resources;
using System;

namespace Common.Extensions
{
	public class ResourceURLAttribute : CustomValidatorAttribute
	{
		public override bool Verify(object value, out string validationError)
		{
			var str = (string)value;
			if(string.IsNullOrEmpty(str))
			{
				validationError = "ResourceURL cannot be empty";
				return false;
			}
			if(str.Length > 512)
			{
				validationError = "ResourceURL cannot be longer than 512 characters";
				return false;
			}
			if(Uri.EscapeUriString(str) != str)
			{
				validationError = "String was not escaped. Expected: " + Uri.EscapeUriString(str);
				return false;
			}
			validationError = null;
			return true;
		}
	}
}
