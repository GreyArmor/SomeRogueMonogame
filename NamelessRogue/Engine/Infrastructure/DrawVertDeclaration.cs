using ImGuiNET;
using SharpDX.Direct3D11;
using SharpDX.DXGI;


namespace NamelessRogue.Engine.Infrastructure
{
	public static class DrawVertDeclaration
	{
		public static readonly InputElement[] Declaration;

		public static readonly int Size;

		static DrawVertDeclaration()
		{
			unsafe { Size = sizeof(ImDrawVert); }

			Declaration =  new InputElement[] {
			  new InputElement("POSITION", 0, Format.R32G32_Float, 0),
			  new InputElement("TEXCOORD", 0, Format.R32G32_Float, sizeof(float) * 2, 0),
			  new InputElement("COLOR", 0, Format.R32G32B32A32_Float, sizeof(float) * 4, 0),
			};
		}
	}
}