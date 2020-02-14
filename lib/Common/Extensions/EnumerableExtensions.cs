using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Common.Extensions
{
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Select a random element from a collection
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
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
			return list.Count == 0 ? default : list[r.Next(0, list.Count)];
		}

		public static IEnumerable<IEnumerable<T>> ChunkRandom<T>(this IEnumerable<T> source, int chunksize)
		{
			var r = new Random();
			while (source.OrderBy(_ => r.Next()).Any())
			{
				yield return source.Take(chunksize);
				source = source.Skip(chunksize);
			}
		}

		public static IEnumerable<T> ConcatenateCollection<T>(this IEnumerable<IEnumerable<T>> sequences)
		{
			return sequences.SelectMany(x => x);
		}

		public static T2 Transpose<T, T2>(this IEnumerable<T> sequence, Func<IEnumerable<T>, T2> function)
		{
			return function(sequence);
		}
	}
}
