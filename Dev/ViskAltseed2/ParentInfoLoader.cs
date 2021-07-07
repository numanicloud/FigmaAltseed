using System.Collections.Generic;
using System.Linq;
using Altseed2;
using FigmaVisk.Capability;
using Visklusa.Abstraction.Notation;

namespace ViskAltseed2
{
	internal class ParentInfoLoader
	{
		private readonly Dictionary<string, NodeAnalysis> _allNode;

		public ParentInfoLoader(IEnumerable<NodeAnalysis> idToNode)
		{
			_allNode = idToNode.Select(x => (id: x.Source.GetCapability<FigmaId>(), x))
				.Where(x => x.id != null)
				.ToDictionary(x => x.id!.NodeId, x => x.x);
		}

		public Dictionary<string, Node> ApplyFamilyShip()
		{
			var map = GetDefaultFamilyMap();
			UpdateFamilyShip(map);

			foreach (var (key, value) in map)
			{
				if (value.Parent is null)
				{
					ApplyAltseedNesting(value);
				}
			}

			return map.Where(x => x.Value.Parent is null)
				.SelectMany(x => GetNodeMap(x.Value))
				.GroupBy(x => x.Item1)
				.ToDictionary(x => x.First().Item1, x => x.First().Item2);
		}

		private Dictionary<string, FamilyShipNode> GetDefaultFamilyMap()
		{
			var result = new Dictionary<string, FamilyShipNode>();
			foreach (var (id, analysis) in _allNode)
			{
				if (analysis.Source.GetCapability<FigmaId>() is {} figmaId)
				{
					result[id] = new FamilyShipNode(figmaId.Name, analysis);
				}
			}

			return result;
		}

		private void UpdateFamilyShip(Dictionary<string, FamilyShipNode> defaultValue)
		{
			foreach (var (key, node) in defaultValue)
			{
				if (node.Analysis.Source.GetCapability<FamilyShip>() is {} family)
				{
					var parent = defaultValue[family.ParentsNodeId];

					parent.Children.Add(node);
					defaultValue[key].Parent = parent;
				}
			}
		}

		private void ApplyAltseedNesting(FamilyShipNode root)
		{
			foreach (var child in root.Children)
			{
				root.Analysis.Node.AddChildNode(child.Analysis.Node);

				if (root.Analysis.Source.GetCapability<BoundingBox>() is { } box
					&& child.Analysis.Node is TransformNode transform)
				{
					transform.Position -= new Vector2F(box.X, box.Y);
				}

				ApplyAltseedNesting(child);
			}
		}

		private IEnumerable<(string, Node)> GetNodeMap(FamilyShipNode root)
		{
			yield return (root.Name, root.Analysis.Node);

			foreach (var child in root.Children)
			{
				foreach (var childMap in GetNodeMap(child))
				{
					yield return (root.Name + "/" + childMap.Item1, childMap.Item2);
				}
			}
		}
	}

	internal class FamilyShipNode
	{
		public FamilyShipNode(string name, NodeAnalysis analysis)
		{
			Name = name;
			Analysis = analysis;
		}

		public string Name { get; set; }
		public NodeAnalysis Analysis { get; }
		public FamilyShipNode? Parent { get; set; }
		public List<FamilyShipNode> Children { get; set; } = new();
	}

	internal record NodeAnalysis(Element Source, Node Node);
}
