using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Stats;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class TurnManagementSystem : ISystem
    {


        public void Update(long gameTime, NamelessGame namelessGame)
        {

            var playerEntity = namelessGame.GetEntitiesByComponentClass<Player>().First();
            HasTurn hasTurn = playerEntity.GetComponentOfType<HasTurn>();

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

            if (hasTurn != null)
            {
                var ap = playerEntity.GetComponentOfType<ActionPoints>();
                if (ap.Points < 100)
                {
                    playerEntity.RemoveComponentOfType<HasTurn>();
                }

                return;
            }

            foreach (var entity in namelessGame.GetEntities())
            {
                var ap = entity.GetComponentOfType<ActionPoints>();
                if (ap != null)
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
                        HasTurn entityTurn = entity.GetComponentOfType<HasTurn>();
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
