using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.shell;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using Tile = NamelessRogue.Engine.Components.ChunksAndTiles.Tile;

namespace NamelessRogue.Engine._3DUtility
{
    public static class ChunkGeometryGeneratorWeb
    {

        static List<Vector3> _points = new List<Vector3>();
        static List<int> _indices = new List<int>();

        static ChunkGeometryGeneratorWeb()
        {
            _points.Add(new Vector3(0, 0, 0));
            _points.Add(new Vector3(1, 0, 0));

            _indices.AddRange(new int[6] { 0, 1, 2, 2, 3, 0 });
        }

        /// <summary>
        /// Generate 3D model for a chunk
        /// </summary>
        /// <param name="chunkToGenerate"></param>
        /// <param name="chunks"></param>
        /// <returns></returns>
        /// 
        static bool firstTime = true;
        static Point originalPointForTest;
        static Matrix moveToZero = Matrix.Translation(-(new Vector3(1, 1, 0) / 2));

        static Random random = new Random();
        public static Geometry3D GenerateChunkModelTiles(NamelessGame game, Point chunkToGenerate, ChunkData chunks, out TerrainGeometry3D terrainGeometry)
        {

            var result = new Geometry3D();
            var terrainGeometryResult = new TerrainGeometry3D();
            Vector4[,] textureData = new Vector4[Constants.ChunkSize, Constants.ChunkSize];
            var chunk = chunks.Chunks[chunkToGenerate];

            var currentCorner = chunk.WorldPositionBottomLeftCorner;
            if (firstTime)
            {
                firstTime = false;
                originalPointForTest = currentCorner;
            }

            Queue<Vertex3D> vertices = new Queue<Vertex3D>();

            var terrainVertices = new Queue<TerrainVertex>();

            Queue<int> indices = new Queue<int>();
            var resolution = Constants.ChunkSize;
            var transformedPoints = new Queue<Vector3>();
            var transformedPointsNormals = new Queue<Vector3>();
            var textureCoordinates = new Queue<Vector2>();
            var colors = new Queue<Vector4>();
            const float worldHeight = 1300;

            //for terrain collion detection
            var tileTriangleAssociations = new List<Point>();

            //bool first = true;
            Vector3 _calculateNormal(Vector3 point, Vector3 neighbor1, Vector3 neightbor2)
            {
                var v1 = point - neighbor1;
                var v2 = point - neightbor2;
                var normal = Vector3.Cross(v1, v2);
                normal.Normalize();
                return normal;
            }

            float _elevationToWorld(float pointElevation)
            {
                float elevation = pointElevation - 0.5f;
                elevation = (elevation) * worldHeight;
                elevation = MathF.Pow(elevation, 2) * 0.005f;
                return elevation;
            }


            Vector3[,] tileNormals = new Vector3[resolution, resolution];
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    Tile tile = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x, chunk.WorldPositionBottomLeftCorner.Y + y);
                    Tile tileE = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x + 1, chunk.WorldPositionBottomLeftCorner.Y + y);
                    Tile tileS = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x, chunk.WorldPositionBottomLeftCorner.Y + y + 1);
                    Tile tileSE = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x + 1, chunk.WorldPositionBottomLeftCorner.Y + y + 1);

                    float elevation = (float)tile.Elevation;
                    float elevationE = (float)tileE.Elevation;
                    float elevationS = (float)tileS.Elevation;
                    float elevationSE = (float)tileSE.Elevation;

                    float elevationMedian = (elevation + elevationE + elevationS + elevationSE) / 4;

                    tile.ElevationVisual = MathF.Pow((float)(elevationMedian - 0.5f) * worldHeight, 2) * 0.005f;

                    //return point adden for normal calculation
                    Vector3 AddPoint(int x, int y, float pointElevation, Tile tile)
                    {

                        float elevation = _elevationToWorld(pointElevation);


                        var vector = (Vector3)Vector3.Transform(new Vector3(
                            x + currentCorner.X - originalPointForTest.X,
                            y + currentCorner.Y - originalPointForTest.Y,
                            elevation), Constants.ScaleDownMatrix);

                        transformedPoints.Enqueue(vector);
                        textureCoordinates.Enqueue(new Vector2((float)x / Constants.ChunkSize, (float)y / Constants.ChunkSize));


                        //colors.Enqueue();
                        return vector;
                    }



                    var tileColor = TerrainLibrary.Terrains[tile.Terrain].Representation.CharColor;
                    textureData[y, x] = tileColor.ToVector4() * (random.Next(7, 9) / 10f);


                    var point = AddPoint(x, y, elevation, tile);
                    var vec1 = AddPoint(x + 1, y, elevationE, tileE);
                    var vec2 = AddPoint(x, y + 1, elevationS, tileS);
                    //AddPoint(x + 1, y + 1, elevationSE, tileSE);




                    var normal = _calculateNormal(point, vec1, vec2);

                    transformedPointsNormals.Enqueue(normal);
                    transformedPointsNormals.Enqueue(normal);
                    transformedPointsNormals.Enqueue(normal);
                    transformedPointsNormals.Enqueue(normal);
                    tileNormals[x, y] = normal;
                    for (int i = 0; i < 6; i++)
                    {
                        tileTriangleAssociations.Add(new Point(x, y));
                    }
                }
            }


            float lastElevation = 0;
            bool newRow = false;
            for (int y = 0; y < resolution + 1; y += 1)
            {
                for (int x = 0; x < resolution + 1; x += 1)
                {
                    var tileX = chunk.WorldPositionBottomLeftCorner.X + x;
                    var tileY = chunk.WorldPositionBottomLeftCorner.Y + y;

                    Tile tile = chunks.GetTile(tileX, tileY);
                    Tile tileS = chunks.GetTile(tileX, tileY + 1);

                    float elevation = (float)tile.Elevation;
                    float elevationN = (float)tileS.Elevation;

                    var normal = tileNormals[Math.Clamp(x, 0, resolution - 1), Math.Clamp(y, 0, resolution - 1)];
                    float pitch = MathF.Abs(MathF.Asin(-normal.Y));
                    float yaw = MathF.Abs(MathF.Atan2(normal.X, normal.Z));

                    if (!terrainVertices.Any())
                    {
                        terrainVertices.Enqueue(new TerrainVertex(_elevationToWorld(elevation), yaw, pitch));
                    }
                    else if (newRow)
                    {
                        newRow = false;
                        terrainVertices.Enqueue(new TerrainVertex(_elevationToWorld(lastElevation), yaw, pitch));
                        terrainVertices.Enqueue(new TerrainVertex(_elevationToWorld(elevation), yaw, pitch));

                    }

                    terrainVertices.Enqueue(new TerrainVertex(_elevationToWorld(elevation), yaw, pitch));
                    terrainVertices.Enqueue(new TerrainVertex(_elevationToWorld(elevationN), yaw, pitch));
                    lastElevation = elevationN;
                }
                newRow = true;
            }





            int triangleCount = 0;
            for (int i = 0; i < transformedPoints.Count - 3; i += 4)
            {
                var index = i;

                indices.Enqueue(index);
                indices.Enqueue(index + 1);
                indices.Enqueue(index + 2);

                indices.Enqueue(index + 3);
                indices.Enqueue(index + 2);
                indices.Enqueue(index + 1);

                triangleCount += 2;
            }
            var points = transformedPoints.ToList();
            while (transformedPoints.Any())
            {
                Vector3 point = transformedPoints.Dequeue();
                var color = new Vector4();
                var vertex = new Systems.Ingame.Vertex3D(point,
                color,
                color, textureCoordinates.Dequeue(), transformedPointsNormals.Dequeue());
                vertices.Enqueue(vertex);
            }  

            Vector4[] tempArr = new Vector4[Constants.ChunkSize * Constants.ChunkSize];

            for (int i = 0; i < Constants.ChunkSize; i++)
            {
                for (int j = 0; j < Constants.ChunkSize; j++)
                {
                    tempArr[i * Constants.ChunkSize + j] = textureData[i, j];
                }
            }

            var textureDescription = new Texture2DDescription()
            {
                Height = Constants.ChunkSize,
                Width = Constants.ChunkSize,
                CpuAccessFlags = CpuAccessFlags.Read,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb,
                Usage = ResourceUsage.Default,
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 0,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0)
            };

            DataStream s = DataStream.Create(tempArr, true, true);
            DataRectangle rect = new DataRectangle(s.DataPointer, Constants.ChunkSize * 4);
            var texture2D = new Texture2D(game.GraphicsDevice, textureDescription, rect);
            s.Close();
            s.Dispose();

            result.Vertices = points;
            result.Indices = indices.ToList();
            result.Bounds = BoundingBox.FromPoints(points.ToArray());
            result.Material = texture2D;
            result.MaterialResourceView = new SharpDX.Direct3D11.ShaderResourceView(game.GraphicsDevice, result.Material);
            result.TriangleTerrainAssociation = tileTriangleAssociations;

            result.Buffer = SharpDX.Direct3D11.Buffer.Create(game.GraphicsDevice, BindFlags.VertexBuffer, vertices.ToArray());
            result.IndexBuffer = SharpDX.Direct3D11.Buffer.Create(game.GraphicsDevice, BindFlags.IndexBuffer, indices.ToArray());
            result.TriangleCount = triangleCount;

            terrainGeometryResult.Buffer = SharpDX.Direct3D11.Buffer.Create(game.GraphicsDevice, BindFlags.VertexBuffer, terrainVertices.ToArray());
            terrainGeometry = terrainGeometryResult;

            var offsetVector =(Vector3)Vector3.Transform(new Vector3(
                currentCorner.X - originalPointForTest.X,
                currentCorner.Y - originalPointForTest.Y,
                            0), Constants.ScaleDownMatrix);

            terrainGeometry.WorldOffset = Matrix.Translation(offsetVector);
            terrainGeometry.VerticesCount = terrainVertices.Count;

            return result;
        }
    }

}
