using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Utility
{
   public static class TargetingHelper
    {
        public static bool IsTargetVisible(IWorldProvider worldProvider, Vector3Int targetPos, Vector3Int entityPos, CharacterStats npcStats)
        {
            var distance = (targetPos - entityPos).Length();
            var visionRange = npcStats.VisionRange.Value;

            bool anyObstacles = false;
            List<Point> line = PointUtil.getLine(entityPos.ToPoint(), targetPos.ToPoint());
            foreach (var point in line)
            {
                var tile = worldProvider.GetTile(point.X, point.Y, entityPos.Z);
                anyObstacles = !tile.IsPassableIgnoringCharacters();
                if (anyObstacles)
                {
                    break;
                }
            }

            return distance <= visionRange && !anyObstacles && targetPos.Z == entityPos.Z;
        }
    }
}
