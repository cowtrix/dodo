// Copyright (C) 2016 by Barend Erasmus, David Jeske and donated to the public domain

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
			if(number.StartsWith("44") && number.Length != 12)
			{
				return false;
			}
			if(!number.StartsWith("44"))
			{
				if(number.StartsWith("07"))
				{
					if(number.Length != 11)
					{
						return false;
					}
					number = "44" + number.Substring(1);
				}
				else if (number.StartsWith("7"))
				{
					if (number.Length != 10)
					{
						return false;
					}
					number = "44" + number;
				}
			}
			return true;
		}
	}
}
