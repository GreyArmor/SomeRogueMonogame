using System.Runtime.Serialization;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Components.Rendering
{
    
    public class Drawable : Component {
        
        public Color BackgroundColor { get; set; }
        public Drawable(char representation, Color charColor, Color backgroundColor = null)
        {
            this.BackgroundColor = backgroundColor;
            if (backgroundColor == null)
            {
                BackgroundColor = new Color();
            }
            Representation=representation;
            CharColor = charColor;
        }

		public Drawable()
		{
		}

		private bool visible = true;
        
        private char Representation;
        
        private Engine.Utility.Color CharColor;

        public char getRepresentation() {
            return Representation;
        }

        public void setRepresentation(char representation) {
            Representation=representation;
        }

        public void setCharColor(Color charColor)
        {
            CharColor = charColor;
        }

        public Color getCharColor() {
            return CharColor;
        }

        public void setVisible(bool visible) {
            this.visible = visible;
        }

        public bool isVisible() {
            return visible;
        }

        public override IComponent Clone()
        {
            return new Drawable(Representation, CharColor,BackgroundColor)
            {
                visible = this.visible
            };
        }
    }
}
