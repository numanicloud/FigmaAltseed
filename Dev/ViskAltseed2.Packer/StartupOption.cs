using System;
using System.Diagnostics.CodeAnalysis;

namespace ViskAltseed2.Packer
{
	internal class StartupOption
	{
		public string? PackagePath { get; set; }
		public string? OutputPath { get; set; }

		[MemberNotNull(nameof(PackagePath), nameof(OutputPath))]
		public void CheckValid()
		{
			if (PackagePath is null || OutputPath is null)
			{
				throw new InvalidOperationException(
					$"Usase:\n{Program.AppName} {nameof(PackagePath)}=\"{PackagePath}\" {nameof(OutputPath)}=\"{OutputPath}\"");
			}
		}
	}
}
