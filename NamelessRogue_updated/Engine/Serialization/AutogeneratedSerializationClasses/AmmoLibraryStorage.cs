
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
    public class AmmoLibraryStorage : IStorage<NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary>
    {
            [FlatBufferItem(0)]  public Guid Id { get; set; }
            [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public List<AmmoTypeStorage> AmmoTypes { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary component)
        {

            this.AmmoTypes = new List<AmmoTypeStorage>(component.AmmoTypes.Cast<AmmoTypeStorage>());

            this.Id = component.Id;

            this.ParentEntityId = component.ParentEntityId;

        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary component)
        {

            component.AmmoTypes = new List<NamelessRogue.Engine.Components.ItemComponents.AmmoType>(this.AmmoTypes.Cast<NamelessRogue.Engine.Components.ItemComponents.AmmoType>());

            component.Id = this.Id;

            component.ParentEntityId = this.ParentEntityId;


        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary (AmmoLibraryStorage thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary result = new NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator AmmoLibraryStorage (NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary  component)
        {
            AmmoLibraryStorage result = new AmmoLibraryStorage();
            result.FillFrom(result);
            return result;
        }

    }

}