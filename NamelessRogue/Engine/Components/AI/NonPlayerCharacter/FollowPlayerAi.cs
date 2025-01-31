using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Components.AI.NonPlayerCharacter
{

    public interface IRounteContainingAI
    {
        public Queue<Point> Route { get; set; }
        Point DestinationPoint { get; set; }
    }

    public class FollowPlayerAi : Component, IRounteContainingAI
    {
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

    public class FollowShootPlayerAi : Component, IRounteContainingAI
    {
        private Queue<Point> route;

        private ShooterAiStates state;
        public FollowShootPlayerAi()
        {
            route = new Queue<Point>();
            state = ShooterAiStates.Idle;
        }

        public Queue<Point> Route
        {
            get { return route; }
            set { route = value; }
        }

        public ShooterAiStates State
        {
            get { return state; }
            set { state = value; }
        }

        public IEntity Target { get; set; }

        public Point DestinationPoint { get; set; }
        public Vector3Int ShootingTarget { get; set; }

        public override IComponent Clone()
        {
            return new FollowShootPlayerAi()
            {
                DestinationPoint = this.DestinationPoint,
                State = this.State,
                Route = this.Route,
                ShootingTarget = this.ShootingTarget
            };
        }
    }


}
