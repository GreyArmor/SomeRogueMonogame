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
        public bool HasShadow { get; internal set; }
        public Engine.Utility.Color CharColor { get; set; }

        public StackedObject(string id, ScreenObjectSource type, Color color, string animationName = "", bool hasShadow = false)
        {
            Id = id;
            Type = type;
            AnimationName = animationName;
            HasShadow = hasShadow;
            CharColor = color;
        }

    }

    public class ScreenTile {
        public ScreenTile()
        {
        }
        
        public void AddObject(string id, ScreenObjectSource type, Color color, bool hasShadow, string animationName = "")
        {
            StackedObjects.Add(new StackedObject(id, type, color, animationName, hasShadow));
        }

        public void AddObjectToBottom(string id, ScreenObjectSource type, Color color, bool hasShadow, string animationName = "")
        {
            StackedObjects.Insert(0, new StackedObject(id, type, color, animationName, hasShadow));
        }
        public List<StackedObject> StackedObjects { get; set; } = new List<StackedObject> ();
        public bool isVisible;
        internal bool isRemembered;
    }
}
