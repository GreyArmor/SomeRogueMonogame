using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public abstract class BaseSystem : ISystem
    {
        private HashSet<IEntity> _registeredEntities = new HashSet<IEntity>();

        public BaseSystem()
        {
            EntityInfrastructureManager.AddSystem(this);
        }

        public abstract HashSet<Type> Signature { get; }
        protected List<IEntity> RegisteredEntities
        {
            get => _registeredEntities.ToList();
        }
        public virtual bool IsEntityMatchesSignature(IEntity entity)
        {
            if (Signature == null || Signature.Count == 0)
            {
                return false;
            }

            var entityComponentTypes = new HashSet<Type>(entity.GetAllComponents().Select(x => x.GetType()));

            foreach (var type in Signature)
            {
                if (!entityComponentTypes.Contains(type))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddEntity(IEntity entity)
        {
            if (!RegisteredEntities.Contains(entity))
            {
                _registeredEntities.Add(entity);
            }
        }

        public void RemoveEntity(IEntity entity)
        {
            _registeredEntities.Remove(entity);
        }

        public abstract void Update(long gameTime, NamelessGame namelessGame);
    }
}
