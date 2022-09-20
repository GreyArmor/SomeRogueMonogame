using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Input;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.PickUpItems
{
    public class PickUpItemScreenSystem : BaseSystem
    {
        public PickUpItemScreenSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(InputComponent));
        }
        public override HashSet<Type> Signature { get; }
        public bool InventoryNeedsUpdate { get; private set; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            /*
            if (InventoryNeedsUpdate)
            {
                UiFactory.PickUpItemsScreen.FillItems(namelessGame);
                InventoryNeedsUpdate = false;
            }

            foreach (var action in UiFactory.PickUpItemsScreen.Actions)
            {
                action.Invoke(this, namelessGame);
            }

            UiFactory.PickUpItemsScreen.Actions.Clear();

            foreach (IEntity entity in RegisteredEntities)
            {
                InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                if (inputComponent != null)
                {
                    var playerEntity = namelessGame.PlayerEntity;
                    foreach (Intent intent in inputComponent.Intents)
                    {
                        switch (intent.Intention)
                        {
                            case IntentEnum.MoveDown:
                            {
                                UiFactory.PickUpItemsScreen.ScrollSelectedTableDown();
                                break;
                            }
                            case IntentEnum.MoveUp:
                            {
                                UiFactory.PickUpItemsScreen.ScrollSelectedTableUp();
                                break;
                            }
                            case IntentEnum.ConetextualHotkeyPressed:
                                var selectedItem =
                                    UiFactory.PickUpItemsScreen.SelectedTable.Items.FirstOrDefault(x =>
                                        x.Hotkey == intent.PressedChar);

                                if (selectedItem != null)
                                {
                                    UiFactory.PickUpItemsScreen.SelectedTable.OnItemClick.Invoke(selectedItem);
                                }

                                break;
                            case IntentEnum.Enter:
                            {
                                UiFactory.PickUpItemsScreen.SelectedTable.OnItemClick.Invoke(UiFactory.PickUpItemsScreen
                                    .SelectedTable.SelectedItem);
                            }
                                break;
                            default:
                                break;
                        }
                    }

                    inputComponent.Intents.Clear();
                }
            }
            */
        }

        internal void BackToGame(NamelessGame game)
        {
            game.ContextToSwitch = ContextFactory.GetIngameContext(game);
        }

        internal void ScheduleUpdate()
        {
            InventoryNeedsUpdate = true;
        }

    }
}
