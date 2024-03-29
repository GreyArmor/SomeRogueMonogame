
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
    public class SettlementStorage : IStorage<NamelessRogue.Engine.Generation.World.Settlement>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public ResorcesStorage Treasury { get; set; }

        [FlatBufferItem(3)]  public ConcreteSettlementStorage Concrete { get; set; }

        [FlatBufferItem(4)]  public String Name { get; set; }

        [FlatBufferItem(5)]  public Vector2Storage Position { get; set; }

        [FlatBufferItem(6)]  public short Representation { get; set; }

        [FlatBufferItem(7)]  public ColorStorage CharColor { get; set; }

        public void FillFrom(NamelessRogue.Engine.Generation.World.Settlement component)
        {

            this.Treasury = component.Treasury;

            this.Concrete = component.Concrete;

            this.Id = component.Id == null ? null : component.Id.ToString();

            this.Name = component.Name;

            this.Position = component.Position;

            this.Representation = (short)component.Representation;

            this.CharColor = component.CharColor;

        }

        public void FillTo(NamelessRogue.Engine.Generation.World.Settlement component)
        {

            component.Treasury = this.Treasury;

            component.Concrete = this.Concrete;

            component.Id = new Guid(this.Id);

            component.Name = this.Name;

            component.Position = this.Position;

            component.Representation = (char)this.Representation;

            component.CharColor = this.CharColor;


        }

        public static implicit operator NamelessRogue.Engine.Generation.World.Settlement (SettlementStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Generation.World.Settlement result = new NamelessRogue.Engine.Generation.World.Settlement();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator SettlementStorage (NamelessRogue.Engine.Generation.World.Settlement  component)
        {
            if(component == null) { return null; }
            SettlementStorage result = new SettlementStorage();
            result.FillFrom(component);
            return result;
        }

    }

}