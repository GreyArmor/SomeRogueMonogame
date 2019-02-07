using System.Runtime.Serialization;
using NamelessRogue.Engine.Engine.Utility;

namespace NamelessRogue.Engine.Engine.Components.Rendering
{
    [DataContract]
    public class Drawable : Component {
        [DataMember]
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

        private bool visible = true;
        [DataMember]
        private char Representation;
        [DataMember]
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
