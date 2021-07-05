using Altseed2;

namespace ViskAltseed2
{
	/// <summary>
	/// 特定のノードが追加の位置情報を持つとき、そのノードの子として付与されるノード。
	/// </summary>
	public class AltPositionTagNode : Node
	{
		public Vector2F Position { get; }

		public AltPositionTagNode(Vector2F position)
		{
			Position = position;
		}
	}
}
