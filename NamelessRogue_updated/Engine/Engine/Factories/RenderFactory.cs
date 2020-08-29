using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Storage.data;

namespace NamelessRogue.Engine.Engine.Factories
{
    public class RenderFactory {
	
        public static Entity CreateViewport(GameSettings settings)
        {
            Entity viewport = new Entity();
            ConsoleCamera camera = new ConsoleCamera(new Point(0,0));
            Screen screen = new Screen(settings.getWidth(),settings.getHeight());
            viewport.AddComponent(camera);
            viewport.AddComponent(screen);
            return viewport;
		
        }
    }
}
