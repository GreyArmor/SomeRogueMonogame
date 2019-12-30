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
                var playerAp = playerEntity.GetComponentOfType<ActionPoints>();
            if (playerAp.Points < 100)
            {
                namelessGame.CurrentGame.Turn++;
                foreach (var entity in namelessGame.GetEntities())
                {
                    var ap = entity.GetComponentOfType<ActionPoints>();
                    if (ap != null)
                    {
                        ap.Points += 100;
                    }
                }
            }
        }
    }
}
