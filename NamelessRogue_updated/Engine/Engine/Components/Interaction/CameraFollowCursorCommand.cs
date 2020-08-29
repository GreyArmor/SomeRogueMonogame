 

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class CameraFollowCursorCommand : Component
    {
        public override IComponent Clone()
        {
           return new CameraFollowCursorCommand();
        }
    }
}
