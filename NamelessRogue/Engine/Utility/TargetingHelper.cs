using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Physical;
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


        /// <summary>
        /// Works in 2d, takes Z level from pointA
        /// </summary>
        /// <param name="worldProvider"></param>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <returns>returns all characters and furniture on path</returns>
        public static List<Tuple<Point,IEntity>> GetCharactersAndFurnitureAlongPath(IWorldProvider worldProvider, Vector3Int pointA, Vector3Int pointB, bool skipFirstTile = true)
        {
            List<Tuple<Point, IEntity>> entities = new List<Tuple<Point, IEntity>>();
            List<Point> line = PointUtil.getLine(pointA.ToPoint(), pointB.ToPoint());
            if(skipFirstTile)
            {
                line = line.Skip(1).ToList();
            }    
            foreach (var point in line)
            {
                var tile = worldProvider.GetTile(point.X, point.Y, pointA.Z);
                var tileEntities = tile.GetEntities();
               foreach(var tileEntity in tileEntities)
                {
                    var furniture = tileEntity.GetComponentOfType<Furniture>();
                    if (furniture != null)
                    {
                        var blocksPath = tileEntity.GetComponentOfType<OccupiesTile>();
                        if(blocksPath != null)
                        {
                            entities.Add(new Tuple<Point, IEntity>(point, tileEntity));
                            continue;
                        }
                    }

                    var character = tileEntity.GetComponentOfType<Character>();
                    if(character!=null)
                    {
                        entities.Add(new Tuple<Point, IEntity>(point, tileEntity));
                    }
                }
            }
            return entities;
        }
    }
}
