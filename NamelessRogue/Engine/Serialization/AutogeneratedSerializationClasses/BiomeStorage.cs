
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
    public class BiomeStorage : IStorage<NamelessRogue.Engine.Infrastructure.Biome>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public BiomesStorage Type { get; set; }

        [FlatBufferItem(3)]  public DrawableStorage Representation { get; set; }

        public void FillFrom(NamelessRogue.Engine.Infrastructure.Biome component)
        {

            this.Type = (BiomesStorage)component.Type;

            this.Representation = component.Representation;

        }

        public void FillTo(NamelessRogue.Engine.Infrastructure.Biome component)
        {

            component.Type = (NamelessRogue.Engine.Generation.World.Biomes)this.Type;

            component.Representation = this.Representation;


        }

        public static implicit operator NamelessRogue.Engine.Infrastructure.Biome (BiomeStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Infrastructure.Biome result = new NamelessRogue.Engine.Infrastructure.Biome();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator BiomeStorage (NamelessRogue.Engine.Infrastructure.Biome  component)
        {
            if(component == null) { return null; }
            BiomeStorage result = new BiomeStorage();
            result.FillFrom(component);
            return result;
        }

    }

}