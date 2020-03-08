namespace NamelessRogue.Engine.Engine.Components.Rendering
{
    public class Screen : Component {
        public int Width { get; }
        public int Height { get; }
        public ScreenTile[,] ScreenBuffer;
        public Screen(int width,int height) {
            Width = width;
            Height = height;
            ScreenBuffer = new ScreenTile[width,height];
            for (int i = 0; i <width;i++)
            {
                for (int j = 0;j<height;j++)
                {
                    ScreenBuffer[i,j]= new ScreenTile();
                }
            }
        }

        public override IComponent Clone()
        {
            return new Screen(Width, Height);
        }
    }
}
