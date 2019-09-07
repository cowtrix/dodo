// Copyright (C) 2016 by Barend Erasmus, David Jeske and donated to the public domain

using System.Text.RegularExpressions;

namespace XR.Dodo
{
	public static class PhoneExtensions
	{
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
