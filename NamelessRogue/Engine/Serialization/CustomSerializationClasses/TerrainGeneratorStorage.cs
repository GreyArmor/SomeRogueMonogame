
// AUTOGENERATED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Serialization.CustomSerializationClasses;
namespace NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses
{
    [FlatBufferTable]
    public class TerrainGeneratorStorage : IStorage<NamelessRogue.Engine.Generation.World.TerrainGenerator>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public InternalRandomStorage Random { get; set; }

        [FlatBufferItem(3)]  public IList<WaterBorderLineStorage> BorderLines { get; set; }

        public void FillFrom(NamelessRogue.Engine.Generation.World.TerrainGenerator component)
        {

            this.Random = component.Random;
            
            this.BorderLines = new List<WaterBorderLineStorage>(component.BorderLines.Select(x=>(WaterBorderLineStorage)x));

        }

        public void FillTo(NamelessRogue.Engine.Generation.World.TerrainGenerator component)
        {
            component.Random = this.Random;
            component.BorderLines = new List<NamelessRogue.Engine.Generation.World.WaterBorderLine>(this.BorderLines.Select(x=>(NamelessRogue.Engine.Generation.World.WaterBorderLine)x));
        }

        public static implicit operator NamelessRogue.Engine.Generation.World.TerrainGenerator (TerrainGeneratorStorage thisType)
        {
            if (thisType == null) { return null; }
            NamelessRogue.Engine.Generation.World.TerrainGenerator result = new NamelessRogue.Engine.Generation.World.TerrainGenerator();
            thisType.FillTo(result);
            result.Init(result.Random);
            return result;
        }

        public static implicit operator TerrainGeneratorStorage (NamelessRogue.Engine.Generation.World.TerrainGenerator  component)
        {
            if (component == null) { return null; }
            TerrainGeneratorStorage result = new TerrainGeneratorStorage();
            result.FillFrom(component);
            return result;
        }

    }

}