using System.Collections.Generic;
using System.Linq;
using Altseed2;

namespace ViskAltseed2
{
	public class VerticalListNode : TransformNode
	{
		private readonly float _itemSpan;
		private float _nextY = 0;
		private Stack<TransformNode> _children = new();

		public VerticalListNode(float itemSpan)
		{
			_itemSpan = itemSpan;
		}

		public void Push(TransformNode node)
		{
			node.Position = new Vector2F(0, _nextY);
			_nextY += node.ContentSize.Y + _itemSpan;
			_children.Push(node);
			AddChildNode(node);
		}

		public void Pop()
		{
			if (!_children.Any())
			{
				return;
			}

			var child = _children.Pop();
			_nextY -= child.ContentSize.Y + _itemSpan;
			RemoveChildNode(child);
		}

		public void Clear()
		{
			var count = _children.Count;
			for (int i = 0; i < count; i++)
			{
				Pop();
			}
		}
	}
}
