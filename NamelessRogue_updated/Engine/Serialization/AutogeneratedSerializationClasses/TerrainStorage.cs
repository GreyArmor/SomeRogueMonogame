
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
    public class TerrainStorage : IStorage<NamelessRogue.Engine.Infrastructure.Terrain>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public TerrainTypesStorage Type { get; set; }

        [FlatBufferItem(3)]  public DrawableStorage Representation { get; set; }

        public void FillFrom(NamelessRogue.Engine.Infrastructure.Terrain component)
        {

            this.Type = (TerrainTypesStorage)component.Type;

            this.Representation = component.Representation;

        }

        public void FillTo(NamelessRogue.Engine.Infrastructure.Terrain component)
        {

            component.Type = (NamelessRogue.Engine.Infrastructure.TerrainTypes)this.Type;

            component.Representation = this.Representation;


        }

        public static implicit operator NamelessRogue.Engine.Infrastructure.Terrain (TerrainStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Infrastructure.Terrain result = new NamelessRogue.Engine.Infrastructure.Terrain();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator TerrainStorage (NamelessRogue.Engine.Infrastructure.Terrain  component)
        {
            if(component == null) { return null; }
            TerrainStorage result = new TerrainStorage();
            result.FillFrom(component);
            return result;
        }

    }

}