using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Factories
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

