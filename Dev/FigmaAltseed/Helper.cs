using System;
using FigmaSharp.Models;
using Color = System.Drawing.Color;

namespace FigmaAltseed
{
	public static class Helper
	{
		public static Color ToDotNet(this FigmaSharp.Color color)
		{
			var (a, r, g, b) = (color.A, color.R, color.G, color.B)
				.Map(d => (int) (d * 255));

			return Color.FromArgb(a, r, g, b);
		}

		public static (T2, T2, T2, T2) Map<T1, T2>(this (T1, T1, T1, T1) tuple, Func<T1, T2> mapper)
		{
			return (mapper(tuple.Item1),
				mapper(tuple.Item2),
				mapper(tuple.Item3),
				mapper(tuple.Item4));
		}

		public static string GetImageAssetPath(this FigmaNode node)
		{
			return node.id.Replace(":", "-").Replace(";", "_") + ".png";
		}
	}
}
