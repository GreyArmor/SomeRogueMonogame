using System.Runtime.Remoting.Messaging;

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class Instanced : Component
    {
        public override IComponent Clone()
        {
            return new Instanced();
        }
    }
}
