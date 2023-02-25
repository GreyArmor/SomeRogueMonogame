
// AUTOGENERATED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Serialization.CustomSerializationClasses;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Components.ChunksAndTiles;

namespace NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses
{
    [FlatBufferTable]
    public class TileStorage : IStorage<NamelessRogue.Engine.Components.ChunksAndTiles.Tile>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public BiomesStorage Biome { get; set; }

        [FlatBufferItem(3)]  public Double Elevation { get; set; }

        [FlatBufferItem(4)]  public TerrainTypesStorage Terrain { get; set; }

        [FlatBufferItem(5)] public IList<EntityStorage> EntitiesOnTile { get; set; }


        public void FillFrom(NamelessRogue.Engine.Components.ChunksAndTiles.Tile component)
        {

            this.Biome = (BiomesStorage)component.Biome;

            this.Elevation = component.Elevation;

            this.Terrain = (TerrainTypesStorage)component.Terrain;

            this.EntitiesOnTile = component.GetEntities().Select(x=>(EntityStorage)x).ToList();

            if (component.GetEntities().Any())
            {
                component.ToString();
            }


        }

        public void FillTo(NamelessRogue.Engine.Components.ChunksAndTiles.Tile component)
        {
            component.Biome = (Generation.World.Biomes)this.Biome;

            component.Elevation = this.Elevation;

            component.Terrain = (TerrainTypes)this.Terrain;

            component.SetEntities(this.EntitiesOnTile.Select(x => (Entity)x).ToList());

            if (EntitiesOnTile.Any())
            {
                EntitiesOnTile.ToString();
            }

        }

        public static implicit operator NamelessRogue.Engine.Components.ChunksAndTiles.Tile (TileStorage thisType)
        {
            if (thisType == null) { return null; }
            NamelessRogue.Engine.Components.ChunksAndTiles.Tile result = new NamelessRogue.Engine.Components.ChunksAndTiles.Tile();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator TileStorage (NamelessRogue.Engine.Components.ChunksAndTiles.Tile  component)
        {
            if (component == null) { return null; }
            TileStorage result = new TileStorage();
            result.FillFrom(component);
            return result;
        }
    }

}