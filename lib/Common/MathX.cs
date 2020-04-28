using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
	public static class MathX
	{
		public static bool ApproxEqual(double first, double second, double precision)
		{
			return Math.Abs(first - second) < precision;
		}
	}
}
