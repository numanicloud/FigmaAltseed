using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FigmaSharp;
using FigmaSharp.Models;
using Svg;
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
			return node.id.Replace(":", "-").Replace(";", "_").TrimStart('I') + ".png";
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

		public static TNode? FindFirst<TNode>(this FigmaCanvas canvas, string id)
			where TNode : FigmaNode
		{
			if (canvas.FindNode(x => x.id == id).FirstOrDefault() is { } node
			    && node is TNode typed)
			{
				return typed;
			}

			return null;
		}

		public static IEnumerable<FigmaNode> TraverseFigma(this FigmaNode root)
		{
			return root.Traverse(node => node.GetChildren<FigmaNode>());
		}

		public static IEnumerable<int> AsEnumerable(this Range range)
		{
			if (range.Start.IsFromEnd || range.End.IsFromEnd)
			{
				throw new InvalidOperationException();
			}

			for (int i = range.Start.Value; i < range.End.Value; i++)
			{
				yield return i;
			}
		}

		public static IEnumerator<int> GetEnumerator(this Range range)
		{
			return range.AsEnumerable().GetEnumerator();
		}

		public static FigmaNode? GetMainSymbol(this FigmaCanvas canvas, FigmaNode instanced)
		{
			if (instanced.id.StartsWith("I"))
			{
				return instanced;
			}

			var split = instanced.id.Split(";");

			for (int i = 0; i < split.Length; i++)
			{
				var id = string.Join(";", split[..i]);

				// 最初に見つかったInstanceに対してコンポーネントを、そしてシンボルを突き止める
				if (canvas.FindNode(x => x.id == id).FirstOrDefault() is { } ancestor
				    && ancestor is FigmaInstance instance)
				{
					var rest = string.Join(";", split[i..]);
					var symbolId = instance.componentId + ";" + rest;

					return canvas.FindNode(x => x.id == symbolId).FirstOrDefault()
					       ?? canvas.FindNode(x => x.id == "I" + symbolId).FirstOrDefault();
				}
			}

			return null;
		}

		public static TValue? TryGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
			where TKey : notnull
		{
			if (dictionary.TryGetValue(key, out var value))
			{
				return value;
			}

			return default;
		}
	}
}
