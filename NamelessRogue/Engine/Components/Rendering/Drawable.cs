using System.Runtime.Serialization;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Components.Rendering
{
    
    public class Drawable : Component {
        
        public Color BackgroundColor { get; set; }
		public string Representation { get => representation; set => representation = value; }
		public bool Visible { get => visible; set => visible = value; }
		public Color CharColor { get => charColor; set => charColor = value; }

		public Drawable(string representationId, Color charColor, Color backgroundColor = null)
        {
            this.BackgroundColor = backgroundColor;
            if (backgroundColor == null)
            {
                BackgroundColor = new Color();
            }
            this.representation= representationId;
            this.charColor = charColor;
        }

		public Drawable()
		{
		}

		private bool visible = true;
        
        private string representation;
        
        private Engine.Utility.Color charColor;


        public override IComponent Clone()
        {
            return new Drawable(representation, charColor, BackgroundColor)
            {
                visible = this.visible
            };
        }
    }
}
