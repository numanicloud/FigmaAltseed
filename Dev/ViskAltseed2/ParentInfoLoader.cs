﻿using System.Collections.Generic;
using System.Linq;
using Altseed2;
using FigmaVisk.Capability;
using Visklusa.Abstraction.Notation;

namespace ViskAltseed2
{
	internal class ParentInfoLoader
	{
		private readonly Dictionary<string, NodeAnalysis> _nodes;

		public ParentInfoLoader(IEnumerable<NodeAnalysis> idToNode)
		{
			_nodes = idToNode.Select(x => (id: x.Source.GetCapability<FigmaId>(), x))
				.Where(x => x.id != null)
				.ToDictionary(x => x.id!.NodeId, x => x.x);
		}

		public void ApplyFamilyShip()
		{
			foreach (var item in _nodes)
			{
				if (item.Value.Source.GetCapability<FamilyShip>() is { } family)
				{
					var parent = _nodes[family.ParentsNodeId];
					var it = item.Value.Node;

					parent.Node.AddChildNode(it);
					if (parent.Source.GetCapability<BoundingBox>() is {} box && it is TransformNode i)
					{
						i.Position -= new Vector2F(box.X , box.Y);
					}
				}
			}
		}
	}

	internal record NodeAnalysis(Element Source, Node Node);
}
