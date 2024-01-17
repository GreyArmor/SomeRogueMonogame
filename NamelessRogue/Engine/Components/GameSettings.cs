namespace NamelessRogue.Engine.Components
{
	public class GameSettings
	{
		//window resolution
		private int widthChars;
		private int heightChars;
		private int fontSize = 32;
		public int Zoom { get; set; } = 1;
		public GameSettings(int defaultWidth, int defaultHeight)
		{
			setWidth(defaultWidth);
			setHeight(defaultHeight);
		}

		public int GetWidthZoomed()
		{
			return widthChars * Zoom;
		}
		public void setWidth(int width)
		{
			widthChars = width;
		}
		public int GetHeight()
		{
			return heightChars;
		}

		public int GetWidth()
		{
			return widthChars;
		}

		public int GetHeightZoomed()
		{
			return heightChars * Zoom;
		}
		public void setHeight(int height)
		{
			heightChars = height;
		}

		public int GetFontSize()
		{
			return fontSize;
		}
		public int GetFontSizeZoomed()
		{
			return fontSize / Zoom;
		}



		public void setFontSize(int fontSize)
		{
			this.fontSize = fontSize;
		}

		public float HudWidth
		{
			get { return 500; }
		}
	}
}
