using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Utility;
using NamelessRogue.FieldOfView;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class VisibilitySystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();
        PermissiveVisibility fov = null;
        Queue<Tile> fovCleaning = new Queue<Tile>(); 
        int playerPosZ = -1;

        public VisibilitySystem()
        {         
        }

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            var world = game.WorldProvider;
            if (fov == null)
            {
               
                fov = new PermissiveVisibility((x, y) =>
                {
                    var worldTile = world.GetTile(x, y, playerPosZ);
                    if (worldTile == null)
                    {
                        return false;
                    }
                    return !world.GetTile(x, y, playerPosZ).GetBlocksVision(game);
                },
                       (x, y) =>
                       {
                           var tile = world.GetTile(x, y, playerPosZ);
                           if (tile != null)
                           {
                               tile.IsVisible = true;
                               tile.IsRemembered = true;
                               fovCleaning.Enqueue(tile);
                           }

                       }, (x, y) => { return (int)Math.Sqrt(x * x + y * y); }
                   );
            }

            while(fovCleaning.TryDequeue(out var tile))
            {
                tile.IsVisible = false;
            }

            var player = game.PlayerEntity;
            var pos = player.GetComponentOfType<Position>();
            playerPosZ = pos.Z;
            fov.Compute(pos.Point.ToPoint(), 40);
        }
    }
}
