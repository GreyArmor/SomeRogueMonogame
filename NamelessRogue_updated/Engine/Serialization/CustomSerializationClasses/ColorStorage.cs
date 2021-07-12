
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Serialization.CustomSerializationClasses;
using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses
{
    [FlatBufferTable]
    public class ColorStorage : IStorage<NamelessRogue.Engine.Utility.Color>, IStorage<Color>
    {
        [FlatBufferItem(0)] public Guid Id { get; set; }
        [FlatBufferItem(1)] public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)] public Single Red { get; set; }

        [FlatBufferItem(3)] public Single Green { get; set; }

        [FlatBufferItem(4)] public Single Blue { get; set; }

        [FlatBufferItem(5)] public Single Alpha { get; set; }

        public void FillFrom(NamelessRogue.Engine.Utility.Color component)
        {
            this.Red = component.Red;

            this.Green = component.Green;

            this.Blue = component.Blue;

            this.Alpha = component.Alpha;
        }

        public void FillFrom(Color component)
        {
            this.Red = component.R;

            this.Green = component.G;

            this.Blue = component.B;

            this.Alpha = component.A;
        }

        public void FillTo(NamelessRogue.Engine.Utility.Color component)
        {
            component.Red = this.Red;

            component.Green = this.Green;

            component.Blue = this.Blue;

            component.Alpha = this.Alpha;
        }

        public void FillTo(Color component)
        {
            component.R = (byte)(this.Red * 255);

            component.G = (byte)(this.Green * 255);

            component.B = (byte)(this.Blue * 255);

            component.A = (byte)(this.Alpha * 255);
        }

        public static implicit operator NamelessRogue.Engine.Utility.Color(ColorStorage thisType)
        {
            NamelessRogue.Engine.Utility.Color result = new NamelessRogue.Engine.Utility.Color();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ColorStorage(NamelessRogue.Engine.Utility.Color component)
        {
            ColorStorage result = new ColorStorage();
            result.FillFrom(component);
            return result;
        }

        public static implicit operator Color(ColorStorage thisType)
        {
            Color result = new Color();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ColorStorage(Color component)
        {
            ColorStorage result = new ColorStorage();
            result.FillFrom(component);
            return result;
        }


    }

}