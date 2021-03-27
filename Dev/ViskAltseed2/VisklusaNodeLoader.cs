using System.Collections.Generic;
using System.Linq;
using Altseed2;
using FigmaVisk.Capability;
using Visklusa.Abstraction.Notation;
using Visklusa.IO;
using Visklusa.JsonAltseed;
using Visklusa.Notation.Json;

namespace ViskAltseed2
{
	public class VisklusaNodeLoader
	{
		public Node[] LoadNodes(string visklusaPath)
		{
			var repo = new JsonCapabilityRepository();
			repo.Register(new JsonCapabilityBase<BoundingBox>(BoundingBox.Id));
			repo.Register(new JsonCapabilityBase<ZOffset>(ZOffset.Id));
			repo.Register(new JsonCapabilityBase<Image>(Image.Id));

			var variant = new JsonAltseedVariant(visklusaPath, repo);
			using var loader = new VisklusaLoader(variant);

			var layout = loader.GetLayout();
			return layout.Elements.Select(ToNode).FilterNull().ToArray();
		}

		private Node? ToNode(Element element)
		{
			if (element.GetCapability<Image>() is {} image
				&& element.GetCapability<BoundingBox>() is {} bound)
			{
				var spriteNode = new SpriteNode()
				{
					Position = new Vector2F(bound.X, bound.Y),
					Texture = Texture2D.Load(image.AssetPath),
				};

				if (element.GetCapability<ZOffset>() is {} zOffset)
				{
					spriteNode.ZOrder = (int)zOffset.Z;
				}

				return spriteNode;
			}

			return null;
		}
	}

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
	}
}
