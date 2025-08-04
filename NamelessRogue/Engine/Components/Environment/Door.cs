 

namespace NamelessRogue.Engine.Components.Environment
{
    public class Door : Component
    {
        public Door()
        {
        }

        public Door(string doorId, bool isTranslucent   )
        {
            DoorId = doorId;
            IsTranslucent = isTranslucent;
        }

        public string DoorId { get; set; }

        public bool IsTranslucent { get; set; }
        public override IComponent Clone()
        {
            return new Door(DoorId, IsTranslucent);
        }
    }
}
