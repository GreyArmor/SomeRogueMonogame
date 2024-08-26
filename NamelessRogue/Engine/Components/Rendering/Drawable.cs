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
        public string TilesetPosition { get; set; }

        public TilesetModifier TilesetModifier { get; set; }
		public Drawable(string representationId, Color charColor, Color backgroundColor = null, string tilesetPosition = "")
        {
            this.BackgroundColor = backgroundColor;
            if (backgroundColor == null)
            {
                BackgroundColor = new Color();
            }
            this.objectID= representationId;
            this.charColor = charColor;
            TilesetPosition = tilesetPosition;
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
