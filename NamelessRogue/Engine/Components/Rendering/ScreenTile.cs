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

        public int AnimationTime { get; }
        public bool HasShadow { get; internal set; }
        public Engine.Utility.Color CharColor { get; set; }
        public bool IsFlying { get; internal set; }
        public bool IsBlurred { get; }

        public StackedObject(string id, ScreenObjectSource type, Color color, string animationName = "", int animationTime = 0, bool hasShadow = false, bool isFlying = false, bool isBlurred = false)
        {
            Id = id;
            Type = type;
            AnimationName = animationName;
            AnimationTime = animationTime;
            HasShadow = hasShadow;
            IsFlying = isFlying;
            IsBlurred = isBlurred;
            CharColor = color;
        }

    }

    public class ScreenTile {
        public ScreenTile()
        {
        }

        public void AddObject(string id, ScreenObjectSource type, Color color, bool hasShadow, bool isFlying, bool isBlurred = false, int animationTime = 0, string animationName = "")
        {
            StackedObjects.Add(new StackedObject(id, type, color, animationName, animationTime, hasShadow, isFlying, isBlurred));
        }

        public void AddObjectToBottom(string id, ScreenObjectSource type, Color color, bool hasShadow, bool isFlying, int animationTime = 0, string animationName = "")
        {
            StackedObjects.Insert(0, new StackedObject(id, type, color, animationName, animationTime, hasShadow, isFlying));
        }
        public List<StackedObject> StackedObjects { get; set; } = new List<StackedObject> ();
        public bool isVisible;
        internal bool isRemembered;
    }
}
