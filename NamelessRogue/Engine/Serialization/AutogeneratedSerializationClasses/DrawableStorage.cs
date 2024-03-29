
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
    public class DrawableStorage : IStorage<NamelessRogue.Engine.Components.Rendering.Drawable>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public ColorStorage BackgroundColor { get; set; }

        [FlatBufferItem(3)]  public short Representation { get; set; }

        [FlatBufferItem(4)]  public Boolean Visible { get; set; }

        [FlatBufferItem(5)]  public ColorStorage CharColor { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Rendering.Drawable component)
        {

            this.BackgroundColor = component.BackgroundColor;

            this.Representation = (short)component.Representation;

            this.Visible = component.Visible;

            this.CharColor = component.CharColor;

            this.Id = component.Id == null ? null : component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId == null ? null : component.ParentEntityId.ToString();

        }

        public void FillTo(NamelessRogue.Engine.Components.Rendering.Drawable component)
        {

            component.BackgroundColor = this.BackgroundColor;

            component.Representation = (char)this.Representation;

            component.Visible = this.Visible;

            component.CharColor = this.CharColor;

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.Rendering.Drawable (DrawableStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Components.Rendering.Drawable result = new NamelessRogue.Engine.Components.Rendering.Drawable();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator DrawableStorage (NamelessRogue.Engine.Components.Rendering.Drawable  component)
        {
            if(component == null) { return null; }
            DrawableStorage result = new DrawableStorage();
            result.FillFrom(component);
            return result;
        }

    }

}