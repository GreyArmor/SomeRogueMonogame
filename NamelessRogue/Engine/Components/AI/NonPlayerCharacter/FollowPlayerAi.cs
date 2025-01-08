using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Components.AI.NonPlayerCharacter
{
    public class FollowPlayerAi : Component {
        private Queue<Point> route;

        private BasicAiStates state;
        public FollowPlayerAi() {
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

        public Point DestinationPoint { get; set; }

        public override IComponent Clone()
        {
           return new FollowPlayerAi()
           {
               DestinationPoint = this.DestinationPoint,
               State = this.State,
               Route = this.Route,
           };
        }
    }
}
