namespace FigmaAltseed.Records
{
	public abstract class FigmaAltseedNode
	{
		public string Name { get; set; } = "";
		public FigmaAltseedNode[] Children { get; set; } = new FigmaAltseedNode[0];
		public AltTransform[] AltTransforms { get; set; } = new AltTransform[0];
	}
}