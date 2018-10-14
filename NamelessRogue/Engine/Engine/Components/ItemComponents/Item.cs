using System;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{
    public class Item : Component {
        private Guid HolderId;
        private Guid entityId;

        public Item(Guid entityId)
        {
            this.entityId = entityId;
        }
        public Guid getHolderId() {
            return HolderId;
        }

        public void setHolderId(Guid holderId) {
            HolderId = holderId;
        }

        public Guid getEntityId() {
            return entityId;
        }
    }
}
