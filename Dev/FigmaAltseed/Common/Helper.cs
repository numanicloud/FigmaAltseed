using System;
using System.Collections.Generic;
using System.Numerics;
using FigmaSharp;
using FigmaSharp.Models;
using Color = System.Drawing.Color;

namespace FigmaAltseed.Common
{
	public static class Helper
	{
		public static Color ToDotNet(this FigmaSharp.Color color)
		{
			var (x, y, z, w) = color;
			var (a, r, g, b) = (x, y, z, w).Map(d => (int) (d * 255));
			return Color.FromArgb(a, r, g, b);
		}

		public static void Deconstruct(this FigmaSharp.Color color,
			out double a, out double r, out double g, out double b)
		{
			(a, r, g, b) = (color.A, color.R, color.G, color.B);
		}

		public static Vector2 ToDotNet(this FigmaSharp.Point point)
		{
			var (x, y) = (point.X, point.Y);
			return new Vector2(x, y);
		}

		public static Vector2 ToDotNet(this FigmaSharp.Size size)
		{
			var (w, h) = (size.Width, size.Height);
			return new Vector2(w, h);
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

		public static IEnumerable<T> FilterNull<T>(this IEnumerable<T?> source)
			where T : notnull
		{
			foreach (var item in source)
			{
				if (item is null)
				{
					continue;
				}

				yield return item;
			}
		}

		public static Rectangle? GetAbsoluteBounding(this FigmaNode node)
		{
			return node is FigmaVector vector ? vector.absoluteBoundingBox
				: node is FigmaFrame frame ? frame.absoluteBoundingBox
				: null;
		}
	}
}
