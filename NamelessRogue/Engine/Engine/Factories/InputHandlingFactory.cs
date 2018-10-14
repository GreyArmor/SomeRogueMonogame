using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Infrastructure;

namespace NamelessRogue.Engine.Engine.Factories
{
    public class InputHandlingFactory {
        public static IEntity CreateInput()
        {
            Entity input = new Entity();
            input.AddComponent(new InputComponent());
            return input;
        }
    }
}

