using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using MonoGame.Extended.Graphics;
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

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class ProjectileRendringSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>() { typeof(Drawable), typeof(ProjectileComponent) };

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            var cameraEntity = game.CameraEntity;
            ConsoleCamera camera = cameraEntity.GetComponentOfType<ConsoleCamera>();
            game.Batch.Begin();
            foreach (IEntity entity in RegisteredEntities)
            {
                var projectileComponent = entity.GetComponentOfType<ProjectileComponent>();
                if (projectileComponent != null)
                {
                    Drawable drawable = entity.GetComponentOfType<Drawable>();
                    var spriteId = drawable.ObjectID;
                    if (SpriteLibrary.SpritesStatic.TryGetValue(spriteId, out var sprite))
                    {
                        int tileHeight = game.GetSettings().GetFontSizeZoomed();
                        int tileWidth = game.GetSettings().GetFontSizeZoomed();
                        var lerpValue = (float)projectileComponent.CurrentFrame / (float)projectileComponent.FramesToReachDestination;
                        var fromVector = projectileComponent.From.ToPoint().ToVector2();
                        var toVector = projectileComponent.To.ToPoint().ToVector2();
                        var interpolatedValue = Vector2.Lerp(fromVector, toVector, lerpValue);
                        Vector2 screenPoint = new Vector2((interpolatedValue.X - camera.Position.X) * tileWidth, (interpolatedValue.Y - camera.Position.Y) * tileHeight);
                        var rect = new Rectangle(screenPoint.ToPoint(), new Vector2(tileWidth, tileHeight).ToPoint());
                        game.Batch.Draw(sprite.TextureRegion, rect, Microsoft.Xna.Framework.Color.White);
                    }
                }
            }
            game.Batch.End();
        }
    }
}
