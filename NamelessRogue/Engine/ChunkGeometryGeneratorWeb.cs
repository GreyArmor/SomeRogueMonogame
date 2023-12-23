using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        static Matrix moveToZero = Matrix.CreateTranslation(-(new Vector3(1, 1, 0) / 2));

        static Random random = new Random();
        public static Geometry3D GenerateChunkModelTiles(Game namelessGame, Point chunkToGenerate, ChunkData chunks, out TerrainGeometry3D terrainGeometry)
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

            Queue<TerrainVertex> terrainVertices = new Queue<TerrainVertex>();

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
           // terrainVertices.Enqueue(new TerrainVertex(0, 0, 0));
           // terrainVertices.Enqueue(new TerrainVertex(0, 0, 0));
            for (int x = 0; x < resolution; x++)
            {
              //  terrainVertices.Enqueue(new TerrainVertex(terrainVertices.Last().vertexHeightYawPitch.X, 0, 0));
                for (int y = 0; y < resolution + 1; y++)
                {

                    Tile tile = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x, chunk.WorldPositionBottomLeftCorner.Y + y);
                    Tile tileE = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x + 1, chunk.WorldPositionBottomLeftCorner.Y + y);


                    float elevation = (float)tile.Elevation;
                    float elevationE = (float)tileE.Elevation;

                    var normal = Vector3.UnitZ;
                    float pitch = MathF.Asin(-normal.Y);
                    float yaw = MathF.Atan2(normal.X, normal.Z);

                    Vector3 normT;
                    normT.X = MathF.Cos(pitch) * MathF.Sin(yaw);
                    normT.Y = MathF.Sin(pitch);
                    normT.Z = MathF.Cos(pitch) * MathF.Cos(yaw);

                    terrainVertices.Enqueue(new TerrainVertex(Constants.ScaleDownCoeficient*10, yaw, pitch));
                    terrainVertices.Enqueue(new TerrainVertex(Constants.ScaleDownCoeficient*10, yaw, pitch));
                }
                terrainVertices.Enqueue(new TerrainVertex(terrainVertices.Last().vertexHeightYawPitch.X, 0, 0));
                terrainVertices.Enqueue(new TerrainVertex(terrainVertices.Last().vertexHeightYawPitch.X, 0, 0));
                terrainVertices.Enqueue(new TerrainVertex(terrainVertices.Last().vertexHeightYawPitch.X, 0, 0));
            }

        


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


                        var vector = Vector3.Transform(new Vector3(
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



                    for (int i = 0; i < 6; i++)
                    {
                        tileTriangleAssociations.Add(new Point(x, y));
                    }
                }
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

            var texture2D = new Texture2D(namelessGame.GraphicsDevice, Constants.ChunkSize, Constants.ChunkSize, false, SurfaceFormat.Vector4);

            Vector4[] tempArr = new Vector4[Constants.ChunkSize * Constants.ChunkSize];

            for (int i = 0; i < Constants.ChunkSize; i++)
            {
                for (int j = 0; j < Constants.ChunkSize; j++)
                {
                    tempArr[i * Constants.ChunkSize + j] = textureData[i, j];
                }
            }

            texture2D.SetData<Vector4>(tempArr, 0, tempArr.Length);


            result.Vertices = points;
            result.Indices = indices.ToList();
            result.Bounds = Microsoft.Xna.Framework.BoundingBox.CreateFromPoints(points);
            result.Material = texture2D;
            result.TriangleTerrainAssociation = tileTriangleAssociations;

            result.Buffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(namelessGame.GraphicsDevice, RenderingSystem3D.VertexDeclaration, vertices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
            result.Buffer.SetData<Vertex3D>(vertices.ToArray());
            result.IndexBuffer = new Microsoft.Xna.Framework.Graphics.IndexBuffer(namelessGame.GraphicsDevice, Microsoft.Xna.Framework.Graphics.IndexElementSize.ThirtyTwoBits, indices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
            result.IndexBuffer.SetData<int>(indices.ToArray());
            result.TriangleCount = triangleCount;


            terrainGeometryResult.Buffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(namelessGame.GraphicsDevice, RenderingSystem3D.TerrainVertexDeclaration, vertices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
            terrainGeometryResult.Buffer.SetData<TerrainVertex>(terrainVertices.Reverse().ToArray());
            terrainGeometry = terrainGeometryResult;



            var offsetVector = Vector3.Transform(new Vector3(
                currentCorner.X - originalPointForTest.X,
                currentCorner.Y - originalPointForTest.Y,
                            0), Constants.ScaleDownMatrix);



            terrainGeometry.WorldOffset = Matrix.CreateRotationZ(SharpDX.MathUtil.DegreesToRadians(180)) * Matrix.CreateTranslation(offsetVector);
            terrainGeometry.VerticesCount = terrainVertices.Count;
           // var chunkVerticesCount = vertices.Count;
            //var arr = terrainVertices.ToArray();

            //var rowIndexEnd = 34;
            //float verticesPerRow = 36; //its 35 because we have a proxy trinagle between each row

            //List<List<float>> rowsX = new List<List<float>>();
            //List<List<float>> rowsY = new List<List<float>>();

           // List<float> rowX = new List<float>();
           // List<float> rowY = new List<float>();

            //for (int vertexId = 0; vertexId < terrainVertices.Count; vertexId++)
            //{
            //    var vert = arr[vertexId];



            //    float rowIndex = vertexId % verticesPerRow;
            //    float clampedIndex = Math.Clamp(rowIndex - 2, 0, rowIndexEnd);

            //    if (clampedIndex / rowIndexEnd == 0)
            //    {
            //        rowsX.Add(rowX.ToList());
            //        rowsY.Add(rowY.ToList());
                   
            //        Debug.Write("rowX=");
            //        foreach (var item in rowX)
            //        {
            //            Debug.Write(" ");
            //            Debug.Write(item);
            //        }
            //        Debug.WriteLine("");
            //        Debug.Write("rowY=");
            //        foreach (var item in rowY)
            //        {
            //            Debug.Write(" ");
            //            Debug.Write(item);
            //        }
            //        Debug.WriteLine("");
            //        rowX.Clear(); rowY.Clear();
            //    }

            //    //float yaw = input.vertexHeightYawPitch.y;
            //    //float pitch = input.vertexHeightYawPitch.z;

            //    Vector4 position = new Vector4();
            //    position.X = MathF.Floor(clampedIndex / 2.0f);
            //    position.Y = clampedIndex % 2.0f;



            //    //new row
            //    //    position.Y += MathF.Floor(vertexId / verticesPerRow);

            //    rowX.Add(position.X); rowY.Add(position.Y);

            //    position.Z = vert.vertexHeightYawPitch.X;
            //    //   Debug.WriteLine($@"rowIndex={rowIndex} position={position}");
            //}




            return result;
        }
    }

}
