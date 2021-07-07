using Altseed2;
using FigmaVisk.Capability;
using System;
using System.Collections.Generic;
using System.Linq;
using Visklusa.Abstraction.Notation;
using Visklusa.IO;
using Visklusa.JsonAltseed;
using Visklusa.Notation.Json;

namespace ViskAltseed2
{
	public class VisklusaNodeLoader
	{
		private readonly Dictionary<(string, int), Font> _fonts = new();
		private Action<TextNode>? _textConfiguration = null;
		private Action<SpriteNode>? _spriteConfiguration = null;

		public LoadResult LoadNodes(string visklusaPath)
		{
			var repo = new JsonCapabilityRepository();
			repo.Register(new JsonCapabilityBase<BoundingBox>(BoundingBox.Id));
			repo.Register(new JsonCapabilityBase<ZOffset>(ZOffset.Id));
			repo.Register(new JsonCapabilityBase<Image>(Image.Id));
			repo.Register(new JsonCapabilityBase<Text>(Text.Id));
			repo.Register(new JsonCapabilityBase<FigmaId>(FigmaId.Id));
			repo.Register(new JsonCapabilityBase<AltPosition>(AltPosition.Id));
			repo.Register(new JsonCapabilityBase<FamilyShip>(FamilyShip.Id));

			var variant = new JsonAltseedVariant(visklusaPath, repo);
			using var loader = new VisklusaLoader(variant);

			var layout = loader.GetLayout();
			var analyzed = layout.Elements
				.Select(x => ToNode(x).WithElement(x))
				.FilterNull()
				.ToArray();

			var parentLoader = new ParentInfoLoader(analyzed);
			var map = parentLoader.ApplyFamilyShip();
			var nodes = analyzed.Select(x => x.Node).ToArray();

			return new LoadResult(nodes, map);
		}

		public void RegisterFont(string fontFamilyName, int fontSize, Font font)
		{
			_fonts[(fontFamilyName, fontSize)] = font;
		}

		public void ConfigureText(Action<TextNode> action) => _textConfiguration = action;
		public void ConfigureSprite(Action<SpriteNode> action) => _spriteConfiguration = action;

		private Node ToNode(Element element)
		{
			if (element.GetCapability<BoundingBox>() is { } box)
			{
				if (element.GetCapability<Image>() is { } image)
				{
					var sprite = CreateSpriteNode(element, box, image);
					_spriteConfiguration?.Invoke(sprite);
					return sprite;
				}

				if (element.GetCapability<Text>() is { } text)
				{
					var textNode = CreateTextNode(element, text, box);
					_textConfiguration?.Invoke(textNode);
					return textNode;
				}

				return new TransformNode()
				{
					Position = new Vector2F(box.X, box.Y)
				};

			}

			return new Node();
		}

		private TextNode CreateTextNode(Element element, Text text, BoundingBox bound2)
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
				Color = new Color((byte)(fill.Red * 255),
					(byte)(fill.Green * 255),
					(byte)(fill.Blue * 255),
					(byte)(fill.Alpha * 255)),
			};

			if (element.GetCapability<ZOffset>() is { } zOffset)
			{
				textNode.ZOrder = (int)zOffset.Z;
			}

			SetCommonCapability(element, textNode);

			return textNode;
		}

		private static SpriteNode CreateSpriteNode(Element element, BoundingBox bound, Image image)
		{
			var spriteNode = new SpriteNode()
			{
				Position = new Vector2F(bound.X, bound.Y),
				Texture = Texture2D.Load(image.AssetPath),
			};

			if (element.GetCapability<ZOffset>() is { } zOffset)
			{
				spriteNode.ZOrder = (int)zOffset.Z;
			}

			SetCommonCapability(element, spriteNode);

			return spriteNode;
		}

		private static void SetCommonCapability(Element element, Node node)
		{
			if (element.GetCapability<AltPosition>() is { } alt)
			{
				node.AddChildNode(new AltPositionTagNode(new Vector2F(alt.X, alt.Y)));
			}


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

		internal static NodeAnalysis? WithElement(this Node? node, Element element)
		{
			return node is { } ? new(element, node) : null;
		}
	}
}
