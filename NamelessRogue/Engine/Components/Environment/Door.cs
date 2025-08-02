 

namespace NamelessRogue.Engine.Components.Environment
{
    public class Door : Component
    {
        public Door()
        {
        }

        public Door(string doorId)
        {
            DoorId = doorId;
        }

        public string DoorId { get; set; }
        public override IComponent Clone()
        {
            return new Door(DoorId);
        }
    }
}
