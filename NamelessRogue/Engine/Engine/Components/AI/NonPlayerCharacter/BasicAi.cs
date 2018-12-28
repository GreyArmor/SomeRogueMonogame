using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Engine.Components.AI.NonPlayerCharacter
{
    public class BasicAi : Component {
        private Queue<Point> route;

        private BasicAiStates state;
        public BasicAi() {
            route = new Queue<Point>();
            state = BasicAiStates.Idle;
        }

        public Queue<Point> Route
        {
            get { return route; }
            set { route = value; }
        }

        public BasicAiStates State
        {
            get { return state; }
            set { state = value; }
        }
    }
}
