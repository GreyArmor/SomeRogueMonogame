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
            ScreenBuffer = new ScreenTile[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
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
