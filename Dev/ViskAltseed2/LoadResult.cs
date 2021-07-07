using System.Collections.Generic;
using Altseed2;

namespace ViskAltseed2
{
	public record LoadResult(Node[] Nodes, Dictionary<string, Node> PathToNode);
}
