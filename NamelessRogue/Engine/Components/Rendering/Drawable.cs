using System.Collections.Generic;
using System.Runtime.Serialization;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Components.Rendering
{
    
    public class Drawable : Component {
        
        public Color BackgroundColor { get; set; }
		public string ObjectID { get => objectID; set => objectID = value; }
		public bool Visible { get => visible; set => visible = value; }
		public Color CharColor { get => charColor; set => charColor = value; }
        public bool CastsShadow { get; set; }
        public bool IsFlying { get; }

        public Drawable(string representationId, Color charColor, Color backgroundColor = null, bool castsShadow = false , bool isFlying = false)
        {
            this.BackgroundColor = backgroundColor;
            if (backgroundColor == null)
            {
                BackgroundColor = new Color();
            }
            this.objectID= representationId;
            this.charColor = charColor;
            CastsShadow = castsShadow;
            IsFlying = isFlying;
        }

		public Drawable()
		{
		}

		private bool visible = true;
        
        private string objectID;
        
        private Engine.Utility.Color charColor;


        public override IComponent Clone()
        {
            return new Drawable(objectID, charColor, BackgroundColor)
            {
                visible = this.visible
            };
        }
    }
}
