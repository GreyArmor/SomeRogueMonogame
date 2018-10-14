using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.Storage.data;

namespace NamelessRogue.Engine.Engine.Utility
{
    public class WorldFovProvider //: ILosBoard
    {

        private IChunkProvider world;
        private Screen screen;
        private ConsoleCamera camera;
        BoundingBox boundingBox;

        public WorldFovProvider(IChunkProvider world, Screen screen, ConsoleCamera camera, GameSettings settings)
        {

            this.world = world;
            this.screen = screen;
            this.camera = camera;
            int camX = camera.getPosition().Y;
            int camY = camera.getPosition().X;
            boundingBox = new BoundingBox(camera.getPosition(),
                new Point(settings.getWidth() + camX, settings.getHeight() + camY));

        }


        public bool contains(int x, int y)
        {
            return boundingBox.isPointInside(x, y);
        }


        public bool isObstacle(int x, int y)
        {
            Tile tileToDraw = world.getTile(x, y);
            return !tileToDraw.getPassable();
        }


        public void visit(int x, int y)
        {
            Point screenPoint = camera.PointToScreen(x, y);
            screen.ScreenBuffer[screenPoint.Y, screenPoint.X].isVisible = true;
        }
    }
}
