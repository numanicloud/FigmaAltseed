using System.Collections.Generic;
using System.Linq;
using FigmaSharp;
using FigmaSharp.Models;
using FigmaVisk.Capability;
using Visklusa.Abstraction.Notation;

namespace FigmaVisk
{
	internal class DocumentAnalyzer
	{
		private record RecursiveContext(int NextId, int Depth);

		public Element[] Analyze(FigmaDocument document)
		{
			var canvas = document.children[0];

			var context = new RecursiveContext(0, 0);
			return canvas.Traverse(
					(node, depth) =>
					{
						context = context with { Depth = depth };

						var element = Convert(node, context);

						context = context with { NextId = context.NextId + 1 };

						return element;
					})
				.FilterNull().ToArray();
		}

		private Element? Convert(FigmaNode node, RecursiveContext context)
		{
			var box = node is FigmaVector vector ? new FigmaBox(vector)
				: node is FigmaFrame frame ? new FigmaBox(frame)
				: null;

			if (box is null)
			{
				return null;
			}

			var capabilities = GetCapabilities(box, context).ToArray();

			return capabilities.Any()
				? new Element(context.NextId, capabilities)
				: null;
		}

		private IEnumerable<ICapability> GetCapabilities(FigmaBox box, RecursiveContext context)
		{
			var bound = box.BoundingBox;
			yield return new BoundingBox(bound.X, bound.Y, bound.Width, bound.Height);

			var paint = new Paint(GetFill(box), GetStroke(box));
			if (paint.Fill != Fill.Blank || paint.Stroke != Stroke.Blank)
			{
				yield return paint;

				if (box.CornerRadius != 0)
				{
					yield return new RoundedRectangle(box.CornerRadius);
				}

				yield return new ZOffset(context.Depth);
			}
		}

		private static Stroke GetStroke(FigmaBox box)
		{
			return box.Stroke is not { } s
				? Stroke.Blank
				: GetStrokeCapability(s.color, box.StrokeWeight);

			static Stroke GetStrokeCapability(Color color, int weight)
			{
				var t = (color.R, color.G, color.B, color.A).ToFloat();
				return new Stroke(t.Item1, t.Item2, t.Item3, t.Item4, weight);
			}
		}

		private static Fill GetFill(FigmaBox box)
		{
			return box.Paint is not { } p ? Fill.Blank
				: p.color is null ? Fill.Blank
				: GetFillCapability(p.color);

			static Fill GetFillCapability(Color color)
			{
				var t = (color.R, color.G, color.B, color.A).ToFloat();
				return new Fill(t.Item1, t.Item2, t.Item3, t.Item4);
			}
		}
	}
}
