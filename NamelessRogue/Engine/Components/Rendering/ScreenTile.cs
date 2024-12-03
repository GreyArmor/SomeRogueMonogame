using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Utility;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Components.Rendering
{

    public enum ScreenObjectSource
    {
        None,
        Tileset,
        StaticSprite,
        AnimatedSprite

    }

    public class StackedObject
    {
        public string Id { get; set; }
        public ScreenObjectSource Type { get; set; }
        public string AnimationName { get; }

        public StackedObject(string id, ScreenObjectSource type, string animationName = "")
        {
            Id = id;
            Type = type;
            AnimationName = animationName;
        }

    }

    public class ScreenTile {
        public ScreenTile()
        {
            CharColor = new Color(0,0,0,0);
            BackGroundColor = new Color(0,0,0,0);
        }
        
        public void AddObject(string id, ScreenObjectSource type, string animationName = "")
        {
            StackedObjects.Add(new StackedObject(id, type, animationName));
        }
        public List<StackedObject> StackedObjects { get; set; } = new List<StackedObject> ();
        public  Engine.Utility.Color CharColor;
        public Engine.Utility.Color BackGroundColor;
        public bool isVisible;
        internal bool isRemembered;
    }
}
