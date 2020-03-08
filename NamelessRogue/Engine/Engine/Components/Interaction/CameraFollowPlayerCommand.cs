 

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class CameraFollowPlayerCommand : Component
    {
        public override IComponent Clone()
        {
            return new CameraFollowPlayerCommand();
        }
    }
}
