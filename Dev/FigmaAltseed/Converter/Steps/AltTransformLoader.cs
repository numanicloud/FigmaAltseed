using System.Linq;
using FigmaAltseed.Common;
using FigmaAltseed.Records;
using FigmaSharp;
using FigmaSharp.Models;
using Svg;

namespace FigmaAltseed.Converter
{
	internal class AltTransformLoader
	{
		public void Load(FigmaEmptyNode nodeTree, FigmaCanvas canvas)
		{
			var alt = canvas.Traverse((FigmaNode node) => node.GetChildren<FigmaNode>())
				.Where(x => x.IsAltTransform());

			var nodes = nodeTree.Traverse((FigmaAltseedNode node) => node.Children)
				.Join(alt,
					node => node.Name,
					node => node.name.RemoveAltTransformTag(),
					(node, figmaNode) => (node, figmaNode));

			foreach (var tuple in nodes)
			{
				var bounding = tuple.figmaNode is FigmaVector vector ? vector.absoluteBoundingBox
					: tuple.figmaNode is FigmaFrame frame ? frame.absoluteBoundingBox
					: null;

				if (bounding is null)
				{
					continue;
				}

				var altTransform = new AltTransform(bounding.Origin.ToDotNet());
				tuple.node.AltTransforms = tuple.node.AltTransforms
					.Append(altTransform)
					.ToArray();
			}
		}
	}
}
