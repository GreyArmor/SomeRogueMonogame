using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using System;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
    [FlatBufferTable]
    public class DateTimeStorage : IStorage<DateTime>
    {
        [FlatBufferItem(0)] public long Ticks { get; set; }

        public void FillFrom(DateTime component)
        {
			Ticks = component.Ticks;
        }

        public void FillTo(DateTime component)
        {
        }

        public static implicit operator DateTime(DateTimeStorage thisType)
        {
            if (thisType == null) { return default; }
            DateTime result = new DateTime(0);
            return result.AddTicks(thisType.Ticks);
        }

        public static implicit operator DateTimeStorage(DateTime component)
        {
            if (component == null) { return null; }
            DateTimeStorage result = new DateTimeStorage();
            result.FillFrom(component);
            return result;
        }
    }
}

