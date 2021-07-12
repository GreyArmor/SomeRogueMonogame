
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
    public class CultureTemplateStorage : IStorage<NamelessRogue.Engine.Generation.World.CultureTemplate>
    {
            [FlatBufferItem(0)]  public Guid Id { get; set; }
            [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public String TemplateName { get; set; }

        [FlatBufferItem(3)]  public String TownNames { get; set; }

        public void FillFrom(NamelessRogue.Engine.Generation.World.CultureTemplate component)
        {

            this.TemplateName = component.TemplateName;

            this.TownNames = component.TownNames;

        }

        public void FillTo(NamelessRogue.Engine.Generation.World.CultureTemplate component)
        {

            component.TemplateName = this.TemplateName;

            component.TownNames = this.TownNames;


        }

        public static implicit operator NamelessRogue.Engine.Generation.World.CultureTemplate (CultureTemplateStorage thisType)
        {
            NamelessRogue.Engine.Generation.World.CultureTemplate result = new NamelessRogue.Engine.Generation.World.CultureTemplate();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator CultureTemplateStorage (NamelessRogue.Engine.Generation.World.CultureTemplate  component)
        {
            CultureTemplateStorage result = new CultureTemplateStorage();
            result.FillFrom(result);
            return result;
        }

    }

}