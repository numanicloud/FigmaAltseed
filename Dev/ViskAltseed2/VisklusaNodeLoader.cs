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
		private readonly Dictionary<(string, int), Font> _fonts = new();

		public Node[] LoadNodes(string visklusaPath)
		{
			var repo = new JsonCapabilityRepository();
			repo.Register(new JsonCapabilityBase<BoundingBox>(BoundingBox.Id));
			repo.Register(new JsonCapabilityBase<ZOffset>(ZOffset.Id));
			repo.Register(new JsonCapabilityBase<Image>(Image.Id));
			repo.Register(new JsonCapabilityBase<Text>(Text.Id));

			var variant = new JsonAltseedVariant(visklusaPath, repo);
			using var loader = new VisklusaLoader(variant);

			var layout = loader.GetLayout();
			return layout.Elements.Select(ToNode).FilterNull().ToArray();
		}

		public void RegisterFont(string fontFamilyName, int fontSize, Font font)
		{
			_fonts[(fontFamilyName, fontSize)] = font;
		}

		private Node? ToNode(Element element)
		{
			if (element.GetCapability<Image>() is {} image
				&& element.GetCapability<BoundingBox>() is {} bound)
			{
				return CreateSpriteNode(element, bound, image);
			}

			if (element.GetCapability<Text>() is {} text
				&& element.GetCapability<BoundingBox>() is {} bound2)
			{
				return CreateTextNode(element, text, bound2);
			}

			return null;
		}

		private Node CreateTextNode(Element element, Text text, BoundingBox bound2)
		{
			var f = _fonts.GetValueOrDefault((text.FontFamily, text.FontSize)) is { } x
				? x
				: Font.LoadDynamicFont("mplus-1c-bold.ttf", text.FontSize);

			var fill = text.Fill;
			var textNode = new TextNode()
			{
				Position = new Vector2F(bound2.X, bound2.Y),
				Text = text.Content,
				Font = f,
				Color = new Color((byte) (fill.Red * 255),
					(byte) (fill.Green * 255),
					(byte) (fill.Blue * 255),
					(byte) (fill.Alpha * 255)),
			};

			if (element.GetCapability<ZOffset>() is { } zOffset)
			{
				textNode.ZOrder = (int) zOffset.Z;
			}

			return textNode;
		}

		private static Node CreateSpriteNode(Element element, BoundingBox bound, Image image)
		{
			var spriteNode = new SpriteNode()
			{
				Position = new Vector2F(bound.X, bound.Y),
				Texture = Texture2D.Load(image.AssetPath),
			};

			if (element.GetCapability<ZOffset>() is { } zOffset)
			{
				spriteNode.ZOrder = (int) zOffset.Z;
			}

			return spriteNode;
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
