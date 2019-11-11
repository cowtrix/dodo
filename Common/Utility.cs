using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Common
{
	public static class Utility
	{
		public static T Random<T>(this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}
			if(!enumerable.Any())
			{
				throw new Exception("Collection cannot be empty");
			}
			// note: creating a Random instance each call may not be correct for you,
			// consider a thread-safe static instance
			var r = new Random();
			var list = enumerable as IList<T> ?? enumerable.ToList();
			return list.Count == 0 ? default(T) : list[r.Next(0, list.Count)];
		}

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

		public static string NeedAmountToString(int amount)
		{
			return amount == int.MaxValue ? "MANY" : amount.ToString();
		}

		public static bool TryParseDateTime(string dt, out DateTime dateTime)
		{
			if(dt == "NOW")
			{
				dateTime = DateTime.MaxValue;
				return true;
			}
			dateTime = default;
			try
			{
				// Format: DD/MM HH:mm
				var split = dt.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				var dateSplit = split[0].Split('/');
				if (dateSplit.Length != 2 || !int.TryParse(dateSplit[0], out var day) || !int.TryParse(dateSplit[1], out var month))
				{
					throw new Exception("Bad date format: " + dt);
				}
				var timeSplit = split[1].Split(new[] { ":", "." }, StringSplitOptions.RemoveEmptyEntries);
				if (timeSplit.Length != 2 || !int.TryParse(timeSplit[0], out var hour) || !int.TryParse(timeSplit[1], out var minute))
				{
					throw new Exception("Bad time format: " + dt);
				}
				dateTime = new DateTime(2019, month, day, hour, minute, 0);
				return true;
			}
			catch (Exception e)
			{
				Logger.Exception(e, "Failed to parse time string: " + dt, nolog:true);
			}
			return false;
		}

		public static string ToDateTimeCode(DateTime timeNeeded)
		{
			if(timeNeeded == DateTime.MaxValue)
			{
				return "NOW";
			}
			return $"{timeNeeded.Day}/{timeNeeded.Month} {timeNeeded.Hour}:{timeNeeded.Minute.ToString("00")}";
		}
	}
}
