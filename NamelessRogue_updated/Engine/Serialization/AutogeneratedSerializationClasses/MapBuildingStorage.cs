
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
    public class MapBuildingStorage : IStorage<NamelessRogue.Engine.Generation.World.BoardPieces.MapBuilding>
    {
            [FlatBufferItem(0)]  public Guid Id { get; set; }
            [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public String Name { get; set; }

        [FlatBufferItem(3)]  public Vector2Storage Position { get; set; }

        [FlatBufferItem(4)]  public Char Representation { get; set; }

        [FlatBufferItem(5)]  public ColorStorage CharColor { get; set; }

        public void FillFrom(NamelessRogue.Engine.Generation.World.BoardPieces.MapBuilding component)
        {

            this.Id = component.Id;

            this.Name = component.Name;

            this.Position = component.Position;

            this.Representation = component.Representation;

            this.CharColor = component.CharColor;

        }

        public void FillTo(NamelessRogue.Engine.Generation.World.BoardPieces.MapBuilding component)
        {

            component.Id = this.Id;

            component.Name = this.Name;

            component.Position = this.Position;

            component.Representation = this.Representation;

            component.CharColor = this.CharColor;


        }

        public static implicit operator NamelessRogue.Engine.Generation.World.BoardPieces.MapBuilding (MapBuildingStorage thisType)
        {
            NamelessRogue.Engine.Generation.World.BoardPieces.MapBuilding result = new NamelessRogue.Engine.Generation.World.BoardPieces.MapBuilding();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator MapBuildingStorage (NamelessRogue.Engine.Generation.World.BoardPieces.MapBuilding  component)
        {
            MapBuildingStorage result = new MapBuildingStorage();
            result.FillFrom(result);
            return result;
        }

    }

}