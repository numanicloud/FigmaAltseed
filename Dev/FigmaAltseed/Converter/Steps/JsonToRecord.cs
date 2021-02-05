using System;
using System.Linq;
using FigmaAltseed.Common;
using FigmaAltseed.Records;
using FigmaSharp;
using FigmaSharp.Models;

namespace FigmaAltseed.Converter.Steps
{
	internal class JsonToRecord
	{
		private readonly FigmaCanvas _canvas;

		public JsonToRecord(FigmaCanvas canvas)
		{
			_canvas = canvas;
		}

		public FigmaEmptyNode GetRecordTree()
		{
			var nodes = _canvas.children
				.Select(ConvertRecursively)
				.ToArray();

			return new FigmaEmptyNode() { Children = nodes };
		}

		private FigmaAltseedNode ConvertRecursively(FigmaNode pivot)
		{
			var children = pivot.GetChildren<FigmaNode>()
				.Select(ConvertRecursively)
				.ToArray();

			if (pivot.visible && !pivot.IsAltTransform())
			{
				var bound = pivot.GetAbsoluteBounding() ?? new Rectangle(0, 0, 0, 0);
				var imagePath = pivot.GetImageAssetPath();

				return new FigmaSpriteNode(imagePath, bound.Origin.ToDotNet(), bound.Size.ToDotNet())
				{
					Name = pivot.name,
					Children = children,
				};
			}

			return new FigmaEmptyNode() {Children = children};
		}

		public class Factory
		{
			public JsonToRecord Create(FigmaCanvas canvas) => new(canvas);
		}
	}
}
