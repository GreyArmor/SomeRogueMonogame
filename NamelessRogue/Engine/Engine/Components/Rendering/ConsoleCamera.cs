using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Engine.Components.Rendering
{
    public class ConsoleCamera : Component {
        private Point position;

        //position is a bottom left corner of camera
        public ConsoleCamera(Point position)
        {
            this.setPosition(position);
        }
	
	
        public Point PointToScreen(Point p)	{
            Point result = new Point();
		
            int cameraX = position.X;
            int cameraY = position.Y;
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
            return position;
        }


        public void setPosition(Point position) {
            this.position = position;
        }
	
        public void setPosition(int x, int y)
        {
            setPosition(new Point(x,y));
        }
    }
}
