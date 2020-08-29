using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Abstraction
{
    public interface ISystem
    {
        void RemoveEntity(IEntity entity);
        void AddEntity(IEntity entity);
        bool IsEntityMatchesSignature(IEntity entity);
        void Update(long gameTime, NamelessGame namelessGame);
    }

}

