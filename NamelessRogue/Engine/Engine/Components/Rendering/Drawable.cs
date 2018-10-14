using NamelessRogue.Engine.Engine.Utility;

namespace NamelessRogue.Engine.Engine.Components.Rendering
{
    public class Drawable : Component {
	
        public Drawable(char representation, Color charColor)
        {
            Representation=representation;
            CharColor = charColor;
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
    }
}
