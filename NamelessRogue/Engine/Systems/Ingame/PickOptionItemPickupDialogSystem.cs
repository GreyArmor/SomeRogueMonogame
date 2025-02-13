using Microsoft.CodeAnalysis.Options;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class PickOptionItemPickupDialogSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out PickOptionDialogOptionCommand command))
            {
                List<IEntity> itemsToPickup = new List<IEntity>();

                foreach (var option in command.DialogPickOptions)
                {
                    itemsToPickup.Add(option.OptionData as IEntity);
                    UIContainer.Instance.PickOptionDialogScreen.Options.Remove(option);
                }

                if(itemsToPickup.Contains(null))
                {
                    itemsToPickup = new List<IEntity>();

                    foreach(var option in UIContainer.Instance.PickOptionDialogScreen.Options)
                    {
                        itemsToPickup.Add(option.OptionData as IEntity);
                    }                 
                    UIContainer.Instance.PickOptionDialogScreen.Options.Clear();
                }

                var pickupItemCommand = new PickUpItemCommand(itemsToPickup, namelessGame.PlayerEntity.GetComponentOfType<ItemsHolder>());
                namelessGame.Commander.EnqueueCommand(pickupItemCommand);

                if (UIContainer.Instance.PickOptionDialogScreen.Options.Count<=1)
                {
                    namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
                }
            }
        }
    }
}
