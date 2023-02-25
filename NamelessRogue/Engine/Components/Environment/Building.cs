using System.Collections.Generic;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Components.Environment
{
    public class Building : Component {
        private List<IEntity> buildingParts = new List<IEntity>();

        public void setBuildingParts(List<IEntity> buildingParts) {
            this.buildingParts = buildingParts;
        }

        public List<IEntity> getBuildingParts() {
            return buildingParts;
        }

        public override IComponent Clone()
        {
            return new Building(){buildingParts = this.buildingParts};
        }
    }
}
