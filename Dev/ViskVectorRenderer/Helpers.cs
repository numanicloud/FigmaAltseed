using System;

namespace ViskVectorRenderer
{
	public static class Helpers
	{
		public static (TR, TR, TR, TR) Map<T, TR>(this (T, T, T, T) tuple, Func<T, TR> mapper)
		{
			return (mapper(tuple.Item1),
				mapper(tuple.Item2),
				mapper(tuple.Item3),
				mapper(tuple.Item4));
		}
	}
}
