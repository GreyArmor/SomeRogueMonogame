using System.Collections.Generic;

namespace NamelessRogue.Engine.Engine.Components.UI
{
    public class SimpleUiElement : Component {
        public SimpleUiElement(SimpleUiElement parent)
        {
            Parent = parent;
        }
        public SimpleUiElement Parent;
        public List<SimpleUiElement> Children = new List<SimpleUiElement> ();
    }
}
