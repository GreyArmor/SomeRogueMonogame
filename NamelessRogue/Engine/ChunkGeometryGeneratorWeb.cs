using FbxSharp;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Utilities;
using BoundingBox = Veldrid.Utilities.BoundingBox;
using Point = Veldrid.Point;
using Tile = NamelessRogue.Engine.Components.ChunksAndTiles.Tile;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;
using VertexPositionTexture = NamelessRogue.Engine.Systems.Ingame.VertexPositionTexture;

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
        static Matrix4x4 moveToZero = Matrix4x4.CreateTranslation(-(new Vector3(1, 1, 0) / 2));

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
                normal = Vector3.Normalize(normal);
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


            TextureDescription textureDescription = TextureDescription.Texture2D(
                (uint)Constants.ChunkSize, (uint)Constants.ChunkSize, 1, 1,
                 PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled);
            var texture2D = game.GraphicsDevice.ResourceFactory.CreateTexture(textureDescription);

            game.GraphicsDevice.UpdateTexture(texture2D, tempArr.ToArray(), 0, 0, 0, (uint)Constants.ChunkSize, (uint)Constants.ChunkSize, 0, 0, 0);


            result.Vertices = points;
            result.Indices = indices.ToList();
            result.Bounds = BoundingBox.CreateFromVertices(points.ToArray());
            result.Material = game.GraphicsDevice.ResourceFactory.CreateTextureView(texture2D);
            result.TriangleTerrainAssociation = tileTriangleAssociations;

            var factory = game.GraphicsDevice.ResourceFactory;

            result.Buffer = factory.CreateBuffer(new BufferDescription((uint)(Vertex3D.Size * vertices.Count), BufferUsage.VertexBuffer));
           
            game.GraphicsDevice.UpdateBuffer(result.Buffer, 0, vertices.ToArray());           

            result.IndexBuffer = factory.CreateBuffer(new BufferDescription((uint)(indices.Count * sizeof(int)), BufferUsage.IndexBuffer));
            game.GraphicsDevice.UpdateBuffer(result.IndexBuffer, 0, indices.ToArray());

            result.TriangleCount = triangleCount;
            
            terrainGeometryResult.Buffer = factory.CreateBuffer(new BufferDescription((uint)(TerrainVertex.Size * terrainVertices.Count), BufferUsage.VertexBuffer));
            terrainGeometry = terrainGeometryResult;

            var offsetVector = (Vector3)Vector3.Transform(new Vector3(
                currentCorner.X - originalPointForTest.X,
                currentCorner.Y - originalPointForTest.Y,
                            0), Constants.ScaleDownMatrix);

            terrainGeometry.WorldOffset = Matrix4x4.CreateTranslation(offsetVector);
            terrainGeometry.VerticesCount = terrainVertices.Count;

            return result;
        }


        public static Geometry3D GenerateChunkModelTilesOld(NamelessGame game, Point chunkToGenerate, ChunkData chunks)
        {

            var result = new Geometry3D();
            Vector4[,] textureData = new Vector4[Constants.ChunkSize, Constants.ChunkSize];
            var chunk = chunks.Chunks[chunkToGenerate];

            var currentCorner = chunk.WorldPositionBottomLeftCorner;
            if (firstTime)
            {
                firstTime = false;
                originalPointForTest = currentCorner;
            }

            Queue<Vertex3D> vertices = new Queue<Vertex3D>();
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
                normal = Vector3.Normalize(normal);
                return normal;
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
                        pointElevation = pointElevation - 0.5f;
                        float elevation = (pointElevation) * worldHeight;
                        elevation = MathF.Pow(elevation, 2) * 0.005f;

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
                    AddPoint(x + 1, y + 1, elevationSE, tileSE);

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

            //var texture2D = new Texture2D(namelessGame.GraphicsDevice, Constants.ChunkSize, Constants.ChunkSize, false, SurfaceFormat.Vector4);

            Vector4[] tempArr = new Vector4[Constants.ChunkSize * Constants.ChunkSize];


            var image = new Image<Rgba32>(Constants.ChunkSize, Constants.ChunkSize);

            for (int i = 0; i < Constants.ChunkSize; i++)
            {
                for (int j = 0; j < Constants.ChunkSize; j++)
                {
                    image[i,j] = new Rgba32(textureData[j, i].X, textureData[j, i].Y, textureData[j, i].Z, 1);
                }
            }
            
            var imageSharpTexture = new ImageSharpTexture(image);
            var texture2D = imageSharpTexture.CreateDeviceTexture(game.GraphicsDevice, game.GraphicsDevice.ResourceFactory);
            result.Bounds = BoundingBox.CreateFromVertices(points.ToArray());
            result.Material = game.GraphicsDevice.ResourceFactory.CreateTextureView(texture2D);
            result.TriangleTerrainAssociation = tileTriangleAssociations;

            result.Vertices = points;                     
            result.Indices = indices.ToList();
            uint verticesCount = (uint)result.Vertices.Count();
            uint indexCount = (uint)result.Indices.Count();
            VertexPositionTexture[] finalVertices = new VertexPositionTexture[verticesCount];

            var verticesArr = vertices.ToArray();
            for (int i = 0; i < verticesCount; i++)
            {
                Vector3 point = result.Vertices[i];
                finalVertices[i] = new VertexPositionTexture(point, verticesArr[i].textureCoordinate);
            }

            var factory = game.GraphicsDevice.ResourceFactory;

            var _worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            Matrix4x4 indentity = Matrix4x4.Identity;
            game.CommandList.UpdateBuffer(_worldBuffer, 0, ref indentity);

            ResourceLayout worldTextureLayout = factory.CreateResourceLayout(
              new ResourceLayoutDescription(
                  new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                  new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                  new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));



            var worldTextureSet = game.GraphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                worldTextureLayout,
                _worldBuffer,
                result.Material,
                game.GraphicsDevice.PointSampler));

            result.WorldTextureSet = worldTextureSet;

            var _vertexBuffer = game.GraphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription((uint)(VertexPositionTexture.SizeInBytes * verticesCount), BufferUsage.VertexBuffer));
            game.GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, finalVertices.Take((int)verticesCount).ToArray());

            var _indexBuffer = game.GraphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription(sizeof(int) * (uint)indexCount, BufferUsage.IndexBuffer));
            game.GraphicsDevice.UpdateBuffer(_indexBuffer, 0, result.Indices.Take((int)indexCount).Select(x => (uint)x).ToArray());

            result.Buffer = _vertexBuffer;
            result.IndexBuffer = _indexBuffer;

            //result.Vertices = points;
            //result.Indices = indices.ToList();
            //result.Bounds = Microsoft.Xna.Framework.BoundingBox.CreateFromPoints(points);
            //result.Material = texture2D;
            //result.TriangleTerrainAssociation = tileTriangleAssociations;

            //result.Buffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(namelessGame.GraphicsDevice, RenderingSystem3D.VertexDeclaration, vertices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
            //result.Buffer.SetData<Vertex3D>(vertices.ToArray());
            //result.IndexBuffer = new Microsoft.Xna.Framework.Graphics.IndexBuffer(namelessGame.GraphicsDevice, Microsoft.Xna.Framework.Graphics.IndexElementSize.ThirtyTwoBits, indices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
            //result.IndexBuffer.SetData<int>(indices.ToArray());
            //result.TriangleCount = triangleCount;
            return result;
        }

    }

}
