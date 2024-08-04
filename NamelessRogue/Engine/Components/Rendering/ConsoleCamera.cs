using System.Diagnostics;


using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Components.Rendering
{
    public class ConsoleCamera : Component {
        private Point position;

		public Point Position { get => position; set => position = value; }

		//position is a bottom left corner of camera
		public ConsoleCamera(Point position)
        {
            this.setPosition(position);
        }

		public ConsoleCamera()
		{
		}

		public Point PointToScreen(Point p)	{
            Point result = new Point();
		
            int cameraX = Position.X;
            int cameraY = Position.Y;
            int worldX = p.X;
            int worldY = p.Y;
            int screenX = worldX - cameraX;
            int screenY = worldY - cameraY;
		
            result.X = (screenX);
            result.Y = (screenY);
            return result;
        }
	
	
        public Point PointToScreen(int x,int y)	{
            return PointToScreen(new Point(x,y));
        }


        public Point getPosition() {
            return Position;
        }


        public void setPosition(Point position) {
            this.Position = position;
            Debug.WriteLine($@"x = {position.X} y={ position.Y }");
        }
	
        public void setPosition(int x, int y)
        {
            setPosition(new Point(x,y));
        }

        public Point GetMouseTilePosition(NamelessGame game)
        {
            return new Point();
            //var state = game.Window.MouseState;
            //var mouseTileX = state.X / game.GetSettings().GetFontSize();
            //var mouseTileY = (game.GetActualHeight() - state.Y) / game.GetSettings().GetFontSize();

            //if (mouseTileX<0||mouseTileX>=game.GetActualCharacterWidth() || mouseTileY < 0 || mouseTileY >= game.GetActualCharacterHeight())
            //{
            //    return new Point(-1,-1);
            //}

            ////Debug.WriteLine($"X = {mouseTileX}; Y = {mouseTileY}");

            //return new Point(Position.X+mouseTileX,Position.Y+mouseTileY);

        }

        public override IComponent Clone()
        {
            return new ConsoleCamera(Position);
        }
    }
}
