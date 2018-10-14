using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Engine.Components.AI.NonPlayerCharacter
{
    public class BasicAi : Component {
        private List<Point> route;

        private BasicAiStates state;
        public BasicAi() {
            route = new List<Point>();
            state = BasicAiStates.Idle;
        }

        public void setRoute(List<Point> route) {
            this.route = route;
        }

        public List<Point> getRoute() {
            return route;
        }

        public void setState(BasicAiStates state) {
            this.state = state;
        }

        public BasicAiStates getState() {
            return state;
        }
    }
}
