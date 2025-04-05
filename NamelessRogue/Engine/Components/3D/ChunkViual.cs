using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace NamelessRogue.Engine.Components._3D
{
    public class ChunkVisual
    {
        public Chunk AssociatedChunk { get; set; }
        public Dictionary<string, AtlasTileData> TilesetData { get; }
        public Texture2D Tileset { get; }
        public Geometry3D Geometry { get; set; } = null;

        public Vector2 Position { get; set; } = Vector2.Zero;

        public ChunkVisual(Chunk AssociatedChunk, Dictionary<string, AtlasTileData> tilesetData, Texture2D tileset)
        {
            this.AssociatedChunk = AssociatedChunk;
            TilesetData = tilesetData;
            Tileset = tileset;
        }

        public void UpdateGeometry(NamelessGame game)
        {
         /*   for (int x = 0; x < Constants.ChunkSize; x++)
            {
                for (int y = 0; y < Constants.ChunkSize; y++)
                {
                    Tile tileToDraw = AssociatedChunk.GetTile(x,y);

                    if (tileToDraw != null)
                    {
                        var terrain = TerrainLibrary.Terrains[tileToDraw.Terrain];

                        var atlasTileData = TilesetData[terrain.Representation.ObjectID];

                        if (atlasTileData == null)
                        {
                            atlasTileData = new AtlasTileData(1, 1);
                        }


                        int tileHeight = game.GetSettings().GetFontSizeZoomed();
                        int tileWidth = game.GetSettings().GetFontSizeZoomed();



                        float textureX = atlasTileData.X * (Constants.tileAtlasTileSize / (float)tileAtlas.Width);
                        float textureY = atlasTileData.Y * (Constants.tileAtlasTileSize / (float)tileAtlas.Height);

                        float textureXend = (atlasTileData.X + 1f) * (Constants.tileAtlasTileSize / (float)tileAtlas.Width);

                        float textureYend = (atlasTileData.Y + 1f) * (Constants.tileAtlasTileSize / (float)tileAtlas.Height);

                        var settings = game.GetSettings();

                        var arrayPosition = (screenPositionX * 4) + (screenPositionY * settings.GetWidthZoomed() * 4);


                        if (atlasTileData.MirrorVerical)
                        {
                            var temp = textureX;
                            textureX = textureXend;
                            textureXend = temp;
                        }
                        if (atlasTileData.MirrorHorizontal)
                        {
                            var temp = textureY;
                            textureY = textureYend;
                            textureYend = temp;
                        }
                        var foregroundvertices = foregroundModel.Vertices;
                        foregroundvertices[arrayPosition] = new Vertex(new Vector3(positionX, positionY, 0), color.ToVector4(),
                        backGroundColor.ToVector4(), new Vector2(textureX, textureY));
                        foregroundvertices[arrayPosition + 1] = new Vertex(new Vector3(positionX + tileWidth, positionY, 0), color.ToVector4(),
                        backGroundColor.ToVector4(), new Vector2(textureXend, textureY));
                        foregroundvertices[arrayPosition + 2] = new Vertex(new Vector3(positionX, positionY + tileHeight, 0), color.ToVector4(),
                        backGroundColor.ToVector4(), new Vector2(textureX, textureYend));
                        foregroundvertices[arrayPosition + 3] = new Vertex(new Vector3(positionX + tileWidth, positionY + tileHeight, 0), color.ToVector4(),
                            backGroundColor.ToVector4(), new Vector2(textureXend, textureYend));
                        var backgroundVertices = backgroundModel.Vertices;
                        backgroundVertices[arrayPosition] = new Vertex(new Vector3(positionX, positionY, 0), color.ToVector4(),
                        backGroundColor.ToVector4(), new Vector2(textureX, textureY));
                        backgroundVertices[arrayPosition + 1] = new Vertex(new Vector3(positionX + tileWidth, positionY, 0), color.ToVector4(),
                        backGroundColor.ToVector4(), new Vector2(textureXend, textureY));
                        backgroundVertices[arrayPosition + 2] = new Vertex(new Vector3(positionX, positionY + tileHeight, 0), color.ToVector4(),
                        backGroundColor.ToVector4(), new Vector2(textureX, textureYend));
                        backgroundVertices[arrayPosition + 3] = new Vertex(new Vector3(positionX + tileWidth, positionY + tileHeight, 0), color.ToVector4(),
                            backGroundColor.ToVector4(), new Vector2(textureXend, textureYend));

                    }

                }
                }
            }
         */
        }

    }
}
