using System.Collections.Generic;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.Environment
{
    public class Building : Component {
        private List<Entity> buildingParts = new List<Entity>();

        public void setBuildingParts(List<Entity> buildingParts) {
            this.buildingParts = buildingParts;
        }

        public List<Entity> getBuildingParts() {
            return buildingParts;
        }

        public override IComponent Clone()
        {
            return new Building(){buildingParts = this.buildingParts};
        }
    }
}
