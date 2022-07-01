using System.Collections.Generic;
using Altseed2;
using FigmaVisk.Capability;
using Visklusa.Abstraction.Notation;

namespace ViskAltseed2
{
	public static class Helpers
	{
		public static IEnumerable<T> FilterNull<T>(this IEnumerable<T?> source) where T : class
		{
			foreach (var item in source)
			{
				if (item is not null)
				{
					yield return item;
				}
			}
		}

		internal static NodeAnalysis? WithElement(this Node? node, Element element)
		{
			return node is { } ? new(element, node) : null;
		}

		internal static Vector2F GetAbsolutePosition(this TransformNode node)
		{
			return node.Parent is TransformNode transform
				? node.Position + GetAbsolutePosition(transform)
				: node.Parent is { } parent
					? node.Position + GetAbsolutePosition(parent)
					: node.Position;
		}

		private static Vector2F GetAbsolutePosition(this Node node)
		{
			return node.Parent is TransformNode transform
				? GetAbsolutePosition(transform)
				: node.Parent is { } parent
					? GetAbsolutePosition(parent)
					: new Vector2F(0, 0);
		}

		public static Color ToColor(this Fill fill)
		{
			return new Color((byte)(fill.Red * 255),
				(byte)(fill.Green * 255),
				(byte)(fill.Blue * 255),
				(byte)(fill.Alpha * 255));
		}
	}
}
