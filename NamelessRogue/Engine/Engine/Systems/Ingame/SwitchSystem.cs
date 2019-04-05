using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Ingame
{
    public class SwitchSystem : ISystem
    {

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in namelessGame.GetEntities())
            {
                ChangeSwitchStateCommand command = entity.GetComponentOfType<ChangeSwitchStateCommand>();
                if (command != null)
                {
                    command.getTarget().setSwitchActive(command.isActive());
                    entity.RemoveComponentOfType<ChangeSwitchStateCommand>();
                }
            }
        }
    }
}
