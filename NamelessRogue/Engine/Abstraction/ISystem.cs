using System;
using System.Collections.Generic;
using SharpDX;
using NamelessRogue.shell;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Abstraction
{
    public interface ISystem
    {
        void RemoveEntity(IEntity entity);
        void AddEntity(IEntity entity);
        bool IsEntityMatchesSignature(IEntity entity);
        void Update(GameTime gameTime, NamelessGame game);
    }

}

