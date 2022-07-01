using Altseed2;
using FigmaVisk.Capability;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Visklusa.Abstraction.Archiver;
using Visklusa.Abstraction.Notation;
using Visklusa.Abstraction.Variant;
using Visklusa.IO;
using Visklusa.Notation.Json;

[assembly: InternalsVisibleTo("ViskAltseed2.Test")]

namespace ViskAltseed2
{
	public class VisklusaNodeLoader
	{
		private Action<TextNode>? _textConfiguration = null;
		private Action<SpriteNode>? _spriteConfiguration = null;

		public LoadResult LoadNodes(Func<JsonCapabilityRepository, IVisklusaVariant> getVariant)
		{
			var repo = new JsonCapabilityRepository();
			repo.Register(new JsonCapabilityBase<BoundingBox>(BoundingBox.Id));
			repo.Register(new JsonCapabilityBase<ZOffset>(ZOffset.Id));
			repo.Register(new JsonCapabilityBase<Image>(Image.Id));
			repo.Register(new JsonCapabilityBase<Text>(Text.Id));
			repo.Register(new JsonCapabilityBase<FigmaId>(FigmaId.Id));
			repo.Register(new JsonCapabilityBase<AltPosition>(AltPosition.Id));
			repo.Register(new JsonCapabilityBase<FamilyShip>(FamilyShip.Id));
			repo.Register(new JsonCapabilityBase<VerticalScroll>(VerticalScroll.Id));
			repo.Register(new JsonCapabilityBase<VerticalList>(VerticalList.Id));

			var variant = getVariant.Invoke(repo);
			using var loader = new VisklusaLoader(variant);
			
			var layout = loader.GetLayout();
			using var archive = variant.GetPackageReader();
			var analyzed = layout.Elements
				.Select(x => ToNode(x, archive).WithElement(x))
				.FilterNull()
				.ToArray();

			var parentLoader = new ParentInfoLoader(analyzed);
			var map = parentLoader.ApplyFamilyShip();
			var nodes = analyzed.Select(x => x.Node).ToArray();

			return new LoadResult(nodes, map);
		}

		public void ConfigureText(Action<TextNode> action) => _textConfiguration = action;
		public void ConfigureSprite(Action<SpriteNode> action) => _spriteConfiguration = action;

		private Node ToNode(Element element, IArchiveReader archive)
		{
			if (element.GetCapability<BoundingBox>() is not { } box)
			{
				return new Node();
			}

			if (element.GetCapability<Image>() is { } image)
			{
				var assetPath = archive.GetAsset(image.AssetPath).FilePath;
				var sprite = CreateSpriteNode(element, box, assetPath);
				_spriteConfiguration?.Invoke(sprite);
				return sprite;
			}

			if (element.GetCapability<Text>() is { } text)
			{
				var textNode = CreateTextNode(element, text, box);
				_textConfiguration?.Invoke(textNode);
				return textNode;
			}

			if (element.GetCapability<Paint>() is {} paint)
			{
				var rectangle = new RectangleNode()
				{
					Position = new Vector2F(box.X, box.Y),
					Scale = new Vector2F(box.Width, box.Height),
					Color = paint.Fill.ToColor()
				};
				SetDrawnCapability(element, rectangle);
				return rectangle;
			}
			
			var transform = new TransformNode()
			{
				Position = new Vector2F(box.X, box.Y)
			};
			SetCommonCapability(element, transform);
			return transform;
		}

		private TextNode CreateTextNode(Element element, Text text, BoundingBox bound2)
		{
			var textNode = new TextNode()
			{
				Position = new Vector2F(bound2.X, bound2.Y),
				Text = text.Content,
				Color = text.Fill.ToColor(),
				FontSize = text.FontSize,
			};

			SetDrawnCapability(element, textNode);

			return textNode;
		}

		private static SpriteNode CreateSpriteNode(Element element, BoundingBox bound, string? assetPath)
		{
			var spriteNode = new SpriteNode()
			{
				Position = new Vector2F(bound.X, bound.Y),
				Texture = Texture2D.Load(assetPath),
			};

			SetDrawnCapability(element, spriteNode);

			return spriteNode;
		}

		private static void SetDrawnCapability<TNode>(Element element, TNode node) where TNode : Node, IDrawn
		{
			if (element.GetCapability<ZOffset>() is { } zOffset)
			{
				node.ZOrder = (int)zOffset.Z;
			}

			SetCommonCapability(element, node);
		}

		private static void SetCommonCapability(Element element, Node node)
		{
			if (element.GetCapability<AltPosition>() is { } alt)
			{
				node.AddChildNode(new AltPositionTagNode(new Vector2F(alt.X, alt.Y)));
			}

			if (element.GetCapability<VerticalList>() is {} vList)
			{
				node.AddChildNode(new VerticalListNode(vList.Spacing){ Position = new Vector2F(0, 0) });
			}

			if (element.GetCapability<VerticalScroll>() is {} vScroll)
			{
				node.AddChildNode(new VerticalScrollView());
			}
		}
	}
}
