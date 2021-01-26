using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using FigmaAltseed.Records;
using FigmaSharp;
using FigmaSharp.Models;
using AppContext = FigmaSharp.AppContext;
using File = System.IO.File;

namespace FigmaAltseed.Converter
{
	internal class JsonToRecord
	{
		public FigmaEmptyNode GetRecordTree(FigmaDocument document)
		{
			var canvas = document.children.First();
			var nodes = canvas.children
				.Select(ConvertRecursively3)
				.ToArray();
			return new FigmaEmptyNode() { Children = nodes };
		}

		private FigmaAltseedNode ConvertRecursively3(FigmaNode pivot)
		{
			var children = pivot.GetChildren<FigmaNode>()
				.Select(ConvertRecursively3)
				.ToArray();

			if (pivot.visible)
			{
				var bound = pivot is FigmaVector vector ? vector.absoluteBoundingBox
					: pivot is FigmaFrame frame ? frame.absoluteBoundingBox
					: new Rectangle(0, 0, 0, 0);

				return new FigmaSpriteNode(pivot.GetImageAssetPath(),
					new Vector2(bound.X, bound.Y),
					new Vector2(bound.Width, bound.Height))
				{
					Children = children,
				};
			}

			return new FigmaEmptyNode() {Children = children};
		}

		// 画像ダウンロードの実装例として残しておく
		private string GetImage(FigmaVector pivot, StartupOption option)
		{
			string GetImageUrl()
			{
				var query = new FigmaImageQuery(option.FileId, new IImageNodeRequest[]
				{
					new ImageNodeRequest(pivot),
				}, option.Token);
				var response = AppContext.Api.GetImage(query);

				var s = response.images.Values.First();
				return s;
			}

			byte[] GetImageData(string url1)
			{
				Console.WriteLine("画像をダウンロード中");
				var request = HttpWebRequest.Create(url1);
				request.Method = "GET";

				var imageResponse = request.GetResponse();
				var length = imageResponse.ContentLength;
				using var reader = new BinaryReader(imageResponse.GetResponseStream());
				var bytes1 = reader.ReadBytes((int)length);
				return bytes1;
			}

			void SaveImage(string filePath1, byte[] bytes2)
			{
				using var file = File.Create(filePath1);
				using var writer = new BinaryWriter(file);
				writer.Write(bytes2);
			}


			var imageId = pivot.id.Replace(":", "-").Replace(";", "_");
			var filePath = $"image{imageId}.png";
			var url = GetImageUrl();
			var bytes = GetImageData(url);
			SaveImage(filePath, bytes);

			return filePath;
		}
	}
}
