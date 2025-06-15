using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class SFXSystem : BaseSystem
    {
        private List<SFXLightningModel> lightningModels = new List<SFXLightningModel>();
        public override HashSet<Type> Signature { get; } = new HashSet<Type>() {};

        private void ProcessSXFCommands(NamelessGame game)
        {
            const int lightningSegmentLenght = 32;
            while (game.Commander.DequeueCommand(out SFXLightningCommand command))
            {
                var directionVector = command.Start.ToPoint().ToVector2() - command.End.ToPoint().ToVector2();
                directionVector.Normalize();
                var angle = MathUtil.AngleBetween(Vector2.UnitX, directionVector);

                var dist = (command.Start - command.End).Length() * lightningSegmentLenght;
                var maxDist = dist;
                while (dist > 0)
                {
                    var lerpValue = (float)dist / maxDist;
                    var fromVector = new Vector2(command.Start.X, command.Start.Y);
                    var toVector = new Vector2(command.End.X, command.End.Y);
                    var interpolatedValue = Vector2.Lerp(fromVector, toVector, lerpValue);
                    lightningModels.Add(new SFXLightningModel() { screenLocation = interpolatedValue, rotation = (float)angle, timeToPlay = 4000 });
                    dist -= lightningSegmentLenght;
                }
            }
        }

        public override void Update(GameTime gameTime, NamelessGame game)
        {

            ProcessSXFCommands(game);

            var cameraEntity = game.CameraEntity;
            ConsoleCamera camera = cameraEntity.GetComponentOfType<ConsoleCamera>();
            game.Batch.Begin();


            int tileHeight = game.GetSettings().GetFontSizeZoomed();

            List<SFXLightningModel> list = lightningModels.ToList();
            for (int lightningIndex = 0; lightningIndex < list.Count; lightningIndex++)
            {
                SFXLightningModel lightningModel = list[lightningIndex];
                var polygonVertices = new List<Vector2>();
                var rotatedPolygon = new List<Vector2>();
                //skip this to let the last segment be in the center of the target
                if (lightningIndex > 0)
                {
                    polygonVertices.Add(new Vector2(0, tileHeight / 2));
                    polygonVertices.Add(new Vector2(tileHeight * 0.2f, (float)(Random.Shared.NextDouble() - 0.5) * (tileHeight / 2) + tileHeight / 2));
                }
                polygonVertices.Add(new Vector2(tileHeight * 0.5f, (float)(Random.Shared.NextDouble() - 0.5) * (tileHeight / 2) + tileHeight / 2));
                polygonVertices.Add(new Vector2(tileHeight * 0.8f, (float)(Random.Shared.NextDouble() - 0.5) * (tileHeight / 2) + tileHeight / 2));
                polygonVertices.Add(new Vector2(tileHeight, tileHeight / 2));

                var origin = new Vector2(tileHeight / 2, tileHeight / 2);
                for (int i = 0; i < polygonVertices.Count; i++)
                {
                    var point = polygonVertices[i];
                    point.RotateAround(origin, MathHelper.ToRadians(lightningModel.rotation));
                    rotatedPolygon.Add(point);
                }

                int tileWidth = game.GetSettings().GetFontSizeZoomed();
                Vector2 screenPoint = new Vector2((lightningModel.screenLocation.X - camera.Position.X) * tileWidth, (lightningModel.screenLocation.Y - camera.Position.Y) * tileHeight);

                for (int i = 0; i < rotatedPolygon.Count - 1; i++)
                {
                    var pointA = rotatedPolygon[i];
                    var pointB = rotatedPolygon[i + 1];
                    game.Batch.DrawLine(screenPoint + pointA, screenPoint + pointB, Microsoft.Xna.Framework.Color.Blue, 4);
                    game.Batch.DrawLine(screenPoint + pointA, screenPoint + pointB, Microsoft.Xna.Framework.Color.White, 2);
                }

                lightningModel.timeToPlay -= gameTime.ElapsedGameTime.Milliseconds;
                if (lightningModel.timeToPlay <= 0)
                {
                    lightningModels.Remove(lightningModel);
                }
            }


            game.Batch.End();
        }
    }
}
