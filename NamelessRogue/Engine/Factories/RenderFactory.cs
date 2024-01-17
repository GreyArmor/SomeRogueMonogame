using Veldrid;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Infrastructure;


namespace NamelessRogue.Engine.Factories
{
    public class RenderFactory {
	
        public static Entity CreateViewport(GameSettings settings)
        {
            Entity viewport = new Entity();
            ConsoleCamera camera = new ConsoleCamera(new Point(0,0));
            Screen screen = new Screen(settings.GetWidthZoomed(), settings.GetHeightZoomed());
            viewport.AddComponent(camera);
            viewport.AddComponent(screen);
            return viewport;
		
        }
    }
}
