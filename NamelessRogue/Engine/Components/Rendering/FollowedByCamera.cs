 

namespace NamelessRogue.Engine.Components.Rendering
{
    public class FollowedByCamera : Component
    {
        public override IComponent Clone()
        {
            return new FollowedByCamera();
        }
    }
}
