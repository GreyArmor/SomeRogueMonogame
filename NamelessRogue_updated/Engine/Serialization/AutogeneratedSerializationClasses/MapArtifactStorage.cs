
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
    public class MapArtifactStorage : IStorage<NamelessRogue.Engine.Generation.World.BoardPieces.MapArtifact>
    {
            [FlatBufferItem(0)]  public Guid Id { get; set; }
            [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 TimeOfLife { get; set; }

        [FlatBufferItem(3)]  public Int32 TimeLeft { get; set; }

        [FlatBufferItem(4)]  public String Name { get; set; }

        [FlatBufferItem(5)]  public Vector2Storage Position { get; set; }

        [FlatBufferItem(6)]  public Char Representation { get; set; }

        [FlatBufferItem(7)]  public ColorStorage CharColor { get; set; }

        public void FillFrom(NamelessRogue.Engine.Generation.World.BoardPieces.MapArtifact component)
        {

            this.TimeOfLife = component.TimeOfLife;

            this.TimeLeft = component.TimeLeft;

            this.Id = component.Id;

            this.Name = component.Name;

            this.Position = component.Position;

            this.Representation = component.Representation;

            this.CharColor = component.CharColor;

        }

        public void FillTo(NamelessRogue.Engine.Generation.World.BoardPieces.MapArtifact component)
        {

            component.TimeOfLife = this.TimeOfLife;

            component.TimeLeft = this.TimeLeft;

            component.Id = this.Id;

            component.Name = this.Name;

            component.Position = this.Position;

            component.Representation = this.Representation;

            component.CharColor = this.CharColor;


        }

        public static implicit operator NamelessRogue.Engine.Generation.World.BoardPieces.MapArtifact (MapArtifactStorage thisType)
        {
            NamelessRogue.Engine.Generation.World.BoardPieces.MapArtifact result = new NamelessRogue.Engine.Generation.World.BoardPieces.MapArtifact();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator MapArtifactStorage (NamelessRogue.Engine.Generation.World.BoardPieces.MapArtifact  component)
        {
            MapArtifactStorage result = new MapArtifactStorage();
            result.FillFrom(result);
            return result;
        }

    }

}