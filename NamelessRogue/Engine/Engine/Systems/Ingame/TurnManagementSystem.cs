using System.Linq;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Ingame
{
    public class TurnManagementSystem : ISystem
    {


        public void Update(long gameTime, NamelessGame namelessGame)
        {

            var playerEntity = namelessGame.GetEntitiesByComponentClass<Player>().First();
            HasTurn hasTurn = playerEntity.GetComponentOfType<HasTurn>();

            if (hasTurn != null)
            {
                var ap = playerEntity.GetComponentOfType<ActionPoints>();
                if (ap.Points < 100)
                {
                    namelessGame.CurrentGame.Turn++;
                }

            }

            foreach (var entity in namelessGame.GetEntities())
            {
                var ap = entity.GetComponentOfType<ActionPoints>();
                if (ap != null)
                {
                    if (ap.Points < 100)
                    {
                        entity.RemoveComponentOfType<HasTurn>();
                    }
                }
            }

           

            foreach (var entity in namelessGame.GetEntities())
            {
                var ap = entity.GetComponentOfType<ActionPoints>();
                HasTurn entityTurn = entity.GetComponentOfType<HasTurn>();
                if (ap != null && entityTurn==null)
                {
                    if (ap.Points < 200)
                    {
                        ap.Points += 100;
                    }

                    if (ap.Points > 200)
                    {
                        ap.Points = 200;
                    }

                    if (ap.Points >= 100)
                    {
                        if (entityTurn == null)
                        {
                            entity.AddComponent(new HasTurn());
                        }
                    }

                }
            }
        }
    }
}
