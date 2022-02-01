namespace NamelessRogue.Engine.Components.Rendering
{
    public class Screen : Component {
        public int Width { get; set; }
        public int Height { get; set; }
        public ScreenTile[,] ScreenBuffer;
        public Screen(int width,int height) {
            Width = width;
            Height = height;
            Resize();
        }


        public void Resize()
        {
            ScreenBuffer = new ScreenTile[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    ScreenBuffer[i, j] = new ScreenTile();
                }
            }
        }


		public Screen()
		{
		}

		public override IComponent Clone()
        {
            return new Screen(Width, Height);
        }
    }
}
