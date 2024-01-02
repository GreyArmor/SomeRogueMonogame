
// AUTOGENERATED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Serialization.CustomSerializationClasses;
using SharpDX;
namespace NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses
{
    [FlatBufferTable]
    public class WaterBorderLineStorage : IStorage<NamelessRogue.Engine.Generation.World.WaterBorderLine>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public IList<PointStorage> Points { get; set; }

        public void FillFrom(NamelessRogue.Engine.Generation.World.WaterBorderLine component)
        {
            
            this.Points = new List<PointStorage>(component.Points.Select(x=>(PointStorage)x));

        }

        public void FillTo(NamelessRogue.Engine.Generation.World.WaterBorderLine component)
        {

            component.Points = new List<Point>(this.Points.Select(x=>(Point)x));


        }

        public static implicit operator NamelessRogue.Engine.Generation.World.WaterBorderLine (WaterBorderLineStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Generation.World.WaterBorderLine result = new NamelessRogue.Engine.Generation.World.WaterBorderLine();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator WaterBorderLineStorage (NamelessRogue.Engine.Generation.World.WaterBorderLine  component)
        {
            if(component == null) { return null; }
            WaterBorderLineStorage result = new WaterBorderLineStorage();
            result.FillFrom(component);
            return result;
        }

    }

}