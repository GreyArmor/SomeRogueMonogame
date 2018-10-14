 

using NamelessRogue.Engine.Engine.Infrastructure;

/**
 * Created by Admin on 16.06.2017.
 */
namespace NamelessRogue.Engine.Engine.Factories
{
    public class UiFactory {
        public Entity CreateHud()
        {
            Entity Hud = new Entity(null);



            return Hud;
        }
    }
}
