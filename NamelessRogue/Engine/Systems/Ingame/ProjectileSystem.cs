using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity = NamelessRogue.Engine.Infrastructure.Entity;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class ProjectileSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>() { typeof(ProjectileComponent)  };

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out CreateProjectileCommand command))
            {
                var projectileEntity = new Entity();
                projectileEntity.AddComponent(new ProjectileComponent(command.From, command.To, 1f));
                projectileEntity.AddComponent(new Drawable("bullet", new Utility.Color(1)));
                projectileEntity.AddComponent(new Position(command.From.X, command.From.Y, command.From.Z));
            }
            var projectileToRemove = new List<IEntity>();
            foreach (var entity in RegisteredEntities) {
                var projectileComponent = entity.GetComponentOfType<ProjectileComponent>();
                if(projectileComponent.CurrentFrame < projectileComponent.FramesToReachDestination)
                {
                    projectileComponent.CurrentFrame++;
                }
                else
                {
                    projectileToRemove.Add(entity);
                }
            }

            foreach (var entity in projectileToRemove)
            {
                namelessGame.RemoveEntity(entity);
            }
        }
    }
}
