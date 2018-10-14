namespace NamelessRogue.Engine.Engine.Components.Rendering
{
    public class Screen : Component {
        public ScreenTile[,] ScreenBuffer;
        public Screen(int width,int height) {
            ScreenBuffer = new ScreenTile[width,height];
            for (int i = 0; i <width;i++)
            {
                for (int j = 0;j<height;j++)
                {
                    ScreenBuffer[i,j]= new ScreenTile();
                }
            }
        }
    }
}
