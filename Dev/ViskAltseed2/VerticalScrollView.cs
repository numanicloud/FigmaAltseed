using System;
using Altseed2;
using System.Reactive.Linq;

namespace ViskAltseed2;

public class VerticalScrollView : Node
{
	private readonly CameraNode _camera;
	private IDisposable _subscription;

	public VerticalScrollView()
	{
		_subscription = Observable.Never<RectF>().Subscribe(f => { });
		_camera = new CameraNode()
		{
			IsColorCleared = true,
			ClearColor = new Color(0, 0, 0, 0)
		};

		var renderTexture = RenderTexture.Create(new Vector2I(1280, 720), TextureFormat.R8G8B8A8_UNORM);
		_camera.TargetTexture = renderTexture;
		var resultSprite = new SpriteNode()
		{
			Texture = renderTexture,
			CameraGroup = 0b1,
			ZOrder = 1000,
			Scale = new Vector2F(273, 203) / new Vector2F(1280, 720),
		};

		AddChildNode(_camera);
		AddChildNode(resultSprite);

		_camera.AddChildNode(new CircleNode()
		{
			Color = new Color(255, 0, 0, 255),
			Radius = 20,
			VertNum = 12,
			CameraGroup = 0b1,
			ZOrder = 1000,
		});
	}

	public void Reset(ulong cameraGroup, IObservable<RectF> onUpdateFocus)
	{
		// CameraはAbsolutePositionではなくPositionに基づいて切り抜き元を決めることに注意

		if (Parent is not TransformNode transform) throw new InvalidOperationException();

		_subscription.Dispose();

		_camera.Scale = transform.ContentSize / new Vector2F(1280, 720);
		_camera.Position = transform.GetAbsolutePosition();
		_camera.Group = cameraGroup;

		_subscription = onUpdateFocus
			.Subscribe(f =>
			{
				var parentArea = new RectF(_camera.Position, transform.ContentSize);
				f = new RectF(f.X, f.Y - 23, f.Width, f.Height + 46);

				if (parentArea.Position.Y > f.Y)
				{
					_camera.Position = parentArea.Position with
					{
						Y = f.Y
					};
				}

				if (parentArea.Position.Y + parentArea.Height < f.Y + f.Height)
				{
					_camera.Position = parentArea.Position with
					{
						Y = f.Y + f.Height - parentArea.Height
					};
				}
			});
	}
}