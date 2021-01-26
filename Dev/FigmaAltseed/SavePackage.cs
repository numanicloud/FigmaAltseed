using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FigmaAltseed
{
	internal class SavePackage
	{
		public void Save(FigmaEmptyNode root)
		{
			var json = JsonSerializer.Serialize(root);
			using var file = File.Create("package");
			using var writer = new StreamWriter(file);

			writer.Write(json);
		}
	}
}
