using SharpDX;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Timers;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static NamelessRogue.Engine.Systems.Ingame.RenderingSystem3D;

namespace NamelessRogue.Engine.Systems
{
    /*internal class SpriteRenderingSystem : BaseSystem
    {
     
        class ModelInstance
        {
            public string modelId;
            public Point2 tile;
            public bool IsFlat;
            public bool AffectedByWind;
        }
        public SpriteRenderingSystem(NamelessGame game)
        {
            spriteBatch = new SpriteBatch(NamelessGame.GraphicsDevice);
            //	spriteFont = Content.Load<SpriteFont>("font");

            spriteEffect = new AlphaTestEffect(NamelessGame.GraphicsDevice)
            {
                VertexColorEnabled = true,
            };
            spriteEffect.World = invertY;
            spriteEffect.View = Matrix.Identity;
        }

        static List<Vector3> _points = new List<Vector3>();
        static List<int> _indices = new List<int>();
        Geometry3D CreateSpriteGeometry(NamelessGame game, SpriteSheetAnimation sheetAnimation)
        {
            _points.Add(new Vector3(1, 1, 0));
            _points.Add(new Vector3(-1, 1, 0));
            _points.Add(new Vector3(-1, -1, 0));
            _points.Add(new Vector3(1, -1, 0));

            _indices.AddRange(new int[6] { 0, 1, 2, 2, 3, 0 });
            var geometry3D = new Geometry3D();
            List<Vertex3D> vertices = new List<Vertex3D>();
            List<Vector2> texCoords = new List<Vector2>();
            var frame = sheetAnimation.CurrentFrame;

            float texelWidth = 1f / frame.Texture.Width;
            float texelHeight = 1f / frame.Texture.Height;

            void _addVertice(Vector3 position, Vector2 textureCoords)
            {
                vertices.Add(new Vertex3D(position, Color.White.ToVector4(), Color.White.ToVector4(), textureCoords, -Vector3.UnitZ));
                texCoords.Add(textureCoords);
            }

            var x = frame.Bounds.X;
            var y = frame.Bounds.Y;
            var sizeX = frame.Bounds.Size.X;
            var sizeY = frame.Bounds.Size.Y;

            _addVertice(_points[3], new Vector2((x + sizeX) * texelWidth, (y + sizeY) * texelHeight));
            _addVertice(_points[2], new Vector2(x * texelWidth, (y + sizeY) * texelHeight));
            _addVertice(_points[1], new Vector2(x * texelWidth, y * texelHeight));
            _addVertice(_points[0], new Vector2((x + sizeX) * texelWidth, y * texelHeight));


            geometry3D.Buffer = new Graphics.VertexBuffer(game.GraphicsDevice, RenderingSystem3D.VertexDeclaration, _points.Count, Graphics.BufferUsage.None);
            geometry3D.Buffer.SetData(vertices.ToArray());
            geometry3D.IndexBuffer = new Graphics.IndexBuffer(game.GraphicsDevice, Graphics.IndexElementSize.ThirtyTwoBits, _indices.Count, Graphics.BufferUsage.None);
            geometry3D.IndexBuffer.SetData<int>(_indices.ToArray());
            geometry3D.TriangleCount = 2;
            geometry3D.Material = frame.Texture;
            return geometry3D;
        }



        public override HashSet<Type> Signature => new HashSet<Type>() {
            typeof(SpriteModel3D), typeof(Position3D)
        };

        private SpriteBatch spriteBatch;
        private AlphaTestEffect spriteEffect;

        Effect effect;

        List<ModelInstance> objectsToDraw = new List<ModelInstance>();
        bool once = true;

        float windCoef = 0f;

        private List<ModelInstance> GetWorldObjectsToDraw(Point positon, IWorldProvider world, out Point2 tilePosition)
        {
            var postionOffsetX = positon.X * Constants.ChunkSize;
            var postionOffsetY = positon.Y * Constants.ChunkSize;
            var objects = new List<ModelInstance>();
            tilePosition = new Point2();

            var chSize = 100 * Constants.ChunkSize;

            for (int x = 0; x < chSize - 1; x++)
            {
                for (int y = 0; y < chSize - 1; y++)
                {
                    Tile tileToDraw = world.GetTile(x + postionOffsetX, y + postionOffsetY);
                    var entities = tileToDraw.GetEntities();
                    foreach (var entity in entities)
                    {
                        // var furniture = entity.GetComponentOfType<Furniture>();
                        var drawable = entity.GetComponentOfType<Drawable>();
                        if (drawable != null)
                        {
                            tilePosition = new Point2(x + postionOffsetX, y + postionOffsetY);
                            switch (drawable.Representation)
                            {
                                case 'T':
                                    {
                                        objects.Add(new ModelInstance() { modelId = "treeEvergreen", tile = tilePosition, AffectedByWind = true });
                                    }
                                    break;
                                case 't':
                                    {
                                        objects.Add(new ModelInstance() { modelId = "smallTree", tile = tilePosition, AffectedByWind = true });
                                    }
                                    break;
                                case 'u':
                                    {
                                        objects.Add(new ModelInstance() { modelId = "stump", tile = tilePosition });
                                    }
                                    break;
                                case 'Q':
                                    {
                                        objects.Add(new ModelInstance() { modelId = "seashells1", tile = tilePosition, IsFlat = true });
                                    }
                                    break;
                                case 'o':
                                    {
                                        objects.Add(new ModelInstance() { modelId = "stone", tile = tilePosition, IsFlat = true });
                                    }
                                    break;
                                case 'S':
                                    {
                                        objects.Add(new ModelInstance() { modelId = "star", tile = tilePosition, IsFlat = true });
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            return objects;
        }
        int offset = Constants.ChunkSize * (300 - Constants.RealityBubbleRangeInChunks);
        Matrix invertY = Matrix.CreateScale(1, -1, 1);
        const float spriteScaleDownCoef = 0.0001f;
        public override void Update(GameTime gameTime, NamelessGame game)
        {

           // return;

            windCoef += Constants.ScaleDownCoeficient*2;

            if (windCoef > 10000)
            {
                windCoef = Constants.ScaleDownCoeficient;
            }

            if (once)
            {
                IEntity worldEntity = game.TimelineEntity;
                IWorldProvider worldProvider = null;
                if (worldEntity != null)
                {
                    worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
                }

                objectsToDraw = GetWorldObjectsToDraw(new Point(300 - Constants.RealityBubbleRangeInChunks, 300 - Constants.RealityBubbleRangeInChunks), worldProvider, out Point2 tilePosition);

                once = false;

                var objectGroups = objectsToDraw.GroupBy(x => x.modelId);

                foreach (var group in objectGroups)
                {
                    var spriteSheet = game.Content.Load<SpriteSheet>($@"Doodads\\{group.First().modelId}.sf", new JsonContentLoader());
                    var spr = new AnimatedSprite(spriteSheet);

                    if (group.First().IsFlat)
                    {
                        spriteCache.Add(group.Key, CreateSpriteGeometry(game, spr.Play("idleFront")));
                    }
                    else
                    {
                        spriteCache.Add(group.Key, CreateSpriteGeometry(game, spr.Play("idleFront")));
                    }
                }

                effect = game.Content.Load<Effect>("TreeShader");
            }

            Camera3D camera = game.PlayerEntity.GetComponentOfType<Camera3D>();

            var player = game.PlayerEntity;
            var frustrum = new BoundingFrustum(camera.View * camera.Projection);

            spriteEffect.View = Matrix.Identity;
            spriteEffect.Projection = camera.Projection;
            spriteBatch.Begin(0, BlendState.AlphaBlend, Graphics.SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, spriteEffect);
            foreach (var entity in RegisteredEntities)
            {
                var pos3d = entity.GetComponentOfType<Position3D>();
                var p = pos3d.Position;

                if (pos3d.WorldPosition == null)
                {
                    pos3d.InitWorldPosition(game, offset);
                }
                var worldPos = pos3d.WorldPosition;
                if (frustrum.Contains(worldPos.Value) == ContainmentType.Contains)
                {
                    var spriteModel = entity.GetComponentOfType<SpriteModel3D>();
                    var viewport = game.GraphicsDevice.Viewport;

                    if (!spriteModel.IdleOnly)
                    {
                        var angle = AngleBetween(new Vector2(camera.Look.X, camera.Look.Y), pos3d.Normal);

                        //default
                        var animationSuffix = "Front";

                        if (angle > -45 && angle < 45)
                        {
                            animationSuffix = "Front";
                        }
                        else if (angle > 45 && angle < 135)
                        {
                            animationSuffix = "Right";
                        }
                        else if (angle > 135 || angle < -135)
                        {
                            animationSuffix = "Back";
                        }
                        else if (angle > -135 && angle < -45f)
                        {
                            animationSuffix = "Left";
                        }

                        spriteModel.Sprite.Play("walk" + animationSuffix);
                        spriteModel.Sprite.Update(GameTime);
                    }

                    Vector3 spritePosition = worldPos.Value;

                    Vector3 viewSpacePosition = Vector3.Transform(spritePosition, camera.View * invertY);

                    if (spriteModel.IdleOnly)
                    {
                        SpriteLibrary.SpritesIdle[spriteModel.SpriteId].Depth = viewSpacePosition.Z;
                        spriteBatch.Draw(SpriteLibrary.SpritesIdle[spriteModel.SpriteId], new Vector2(viewSpacePosition.X, viewSpacePosition.Y), 0, new Vector2(spriteScaleDownCoef, spriteScaleDownCoef));
                    }
                    else
                    {
                        spriteModel.Sprite.Depth = viewSpacePosition.Z;
                        spriteBatch.Draw(spriteModel.Sprite, new Vector2(viewSpacePosition.X, viewSpacePosition.Y), 0, new Vector2(spriteScaleDownCoef, spriteScaleDownCoef));
                    }
                }
            }
            spriteBatch.End();

            effect.Parameters["xViewProjection"].SetValue(camera.View * camera.Projection);
            effect.Parameters["xWorldViewProjection"].SetValue(camera.View * camera.Projection);
            effect.Parameters["xView"].SetValue(camera.View);
            effect.Parameters["xProjection"].SetValue(camera.Projection);
            effect.Parameters["CameraPosition"].SetValue(camera.Position);
            effect.Parameters["CameraUp"].SetValue(camera.Up);
            effect.Parameters["CameraRight"].SetValue(camera.Right);
            effect.Parameters["xViewProjection"].SetValue(camera.Projection * camera.View);
            effect.Parameters["windCoef"].SetValue(new Vector2(windCoef));
            Debug.WriteLine(windCoef);
            var invertedView = Matrix.Invert(camera.View);
            if (camera.View == Matrix.Identity)
            {
                return;
            }




            //return;
            RenderStaticObjects(game, camera);
        }




        Dictionary<string, int> cacheCheckDictionary = new Dictionary<string, int>();
        Dictionary<string, VertexBuffer> instanceBufferCache = new Dictionary<string, VertexBuffer>();
        Dictionary<string, Geometry3D> spriteCache = new Dictionary<string, Geometry3D>();
        Matrix worldtest;
        List<VertexShaderInstanceMatrix> instanceTransforms;

        bool onceMatrix = true;
        Matrix storedFirst = Matrix.Identity;
        private void RenderStaticObjects(NamelessGame game, Camera3D camera)
        {

            effect.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            //effect.GraphicsDevice.BlendState = BlendState.Opaque;

            var device = game.GraphicsDevice;
            effect.CurrentTechnique = effect.Techniques["TextureTechInstanced"];

            effect.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            effect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            effect.GraphicsDevice.RasterizerState = RasterizerState.CullNone;


            var rotZ = Matrix.CreateFromAxisAngle(Vector3.UnitZ, Vector3.Dot(Vector3.UnitX, new Vector3(camera.Look.X, camera.Look.Y, 0)));

            camera.View.Decompose(out var scale, out var rot, out Vector3 translation);

            //avoid setting the same buffer a million times, only transform is changed
            var objectGroups = objectsToDraw.GroupBy(x => x.modelId);

            foreach (var group in objectGroups)
            {
                var rotation = Matrix.Invert(Matrix.CreateFromQuaternion(rot));
                if (!group.First().IsFlat)
                {
                    effect.Parameters["xBillboard"].SetValue(rotation);
                }
                else
                {
                    effect.Parameters["xBillboard"].SetValue(Matrix.Identity);
                }

                if (group.First().AffectedByWind)
                {
                    effect.Parameters["windFlag"].SetValue(true);
                }
                else 
                {
                    effect.Parameters["windFlag"].SetValue(false);
                }


                VertexBuffer instanceBuffer;
                var groupCount = group.Count();
                if (cacheCheckDictionary.TryGetValue(group.Key, out int numberOfInstances) && numberOfInstances == groupCount)
                {
                    instanceBuffer = instanceBufferCache[group.Key];
                }
                else
                {
                    instanceTransforms = new List<VertexShaderInstanceMatrix>();
                    //create insatnce data buffer
                    foreach (var gameObject in group)
                    {
                        var tileToDraw = game.WorldProvider.GetTile((int)gameObject.tile.X, (int)gameObject.tile.Y);
                        var p = gameObject.tile;
                        var position = new Point((int)(p.X - offset), (int)(p.Y - offset));

                        var shiftToTileCenter = Constants.ScaleDownCoeficient / 2;

                        var billboardShiftUp = gameObject.IsFlat? Constants.ScaleDownCoeficient * 0.1f : Constants.ScaleDownCoeficient;
                        var world = Constants.ScaleDownMatrix * Matrix.CreateTranslation(position.X * Constants.ScaleDownCoeficient + shiftToTileCenter, position.Y * Constants.ScaleDownCoeficient + shiftToTileCenter, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient + billboardShiftUp);
                        var worldPos = Vector3.Transform(Vector3.Zero, world);
                        instanceTransforms.Add(new VertexShaderInstanceMatrix(world));
                    }

                    instanceBuffer = new VertexBuffer(device, VertexShaderInstanceInput, instanceTransforms.Count, BufferUsage.WriteOnly);
                    instanceBuffer.SetData(instanceTransforms.ToArray());
                    cacheCheckDictionary[group.Key] = groupCount;
                    instanceBufferCache[group.Key] = instanceBuffer;
                }
                var geometry = spriteCache[group.First().modelId];
                effect.Parameters["tileAtlas"].SetValue(SpriteLibrary.SpritesIdle[group.First().modelId].TextureRegion.Texture);

                //var pos = Vector3.Transform(Vector3.Zero, storedFirst);
                //var screePos = Vector3.Transform(pos, camera.View * camera.Projection);
                //Debug.WriteLine("worldposition in camera" + screePos);
                //var projected = Project(NamelessGame.GraphicsDevice.Viewport, pos, camera.Projection, camera.View, Matrix.Identity);
                //Debug.WriteLine(@$"x ={projected.X} y = {projected.Y}  z= {projected.Z}");


                var bindings = new VertexBufferBinding[2];
                bindings[0] = new VertexBufferBinding(geometry.Buffer);
                bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
                device.SetVertexBuffers(bindings);
                device.Indices = geometry.IndexBuffer;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 2, cacheCheckDictionary[group.Key]);
                }
                //    break;
            }
        }


        //public Vector3 Project(Viewport viewport,  Vector3 source, Matrix projection, Matrix view, Matrix world)
        //{
        //	Matrix matrix = Matrix.Multiply(Matrix.Multiply(world, view), projection);
        //	Vector3 result = Vector3.Transform(source, matrix);
        //	float num = source.X * matrix.M14 + source.Y * matrix.M24 + source.Z * matrix.M34 + matrix.M44;
        //	if (!WithinEpsilon(num, 1f))
        //	{
        //		result.X /= num;
        //		result.Y /= num;
        //		result.Z /= num;
        //	}

        //	result.X = (result.X + 1f) * 0.5f * (float)viewport.Width + (float)viewport.X;
        //	result.Y = (0f - result.Y + 1f) * 0.5f * (float)viewport.Height + (float)viewport.Y;
        //	result.Z = result.Z * (viewport.MaximumDepth - viewport.MinimumDepth) + viewport.MinimumDepth;
        //	return result;
        //}

        //private static bool WithinEpsilon(float a, float b)
        //{
        //	float num = a - b;
        //	if (-1.401298E-45f <= num)
        //	{
        //		return num <= float.Epsilon;
        //	}

        //	return false;
        //}


        //TODO useful utility, move somewhere appropriate
        private double AngleBetween(Vector2 vector1, Vector2 vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }




    }
        */
}
