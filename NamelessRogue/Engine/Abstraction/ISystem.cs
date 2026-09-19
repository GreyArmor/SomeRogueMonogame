using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Abstraction
{
    public interface ISystem
    {
        void RemoveEntity(Entity entity);
        void AddEntity(Entity entity);
        bool IsEntityMatchesSignature(Entity entity);
        void Update(GameTime gameTime, NamelessGame namelessGame);
    }

}

