using SharpDX;
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
using NamelessRogue.Engine.Infrastructure;

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

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            /*
            if (InventoryNeedsUpdate)
            {
                UIController.PickUpItemsScreen.FillItems(game);
                InventoryNeedsUpdate = false;
            }

            foreach (var action in UIController.PickUpItemsScreen.Actions)
            {
                action.Invoke(this, game);
            }

            UIController.PickUpItemsScreen.Actions.Clear();

            foreach (IEntity entity in RegisteredEntities)
            {
                InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                if (inputComponent != null)
                {
                    var playerEntity = game.PlayerEntity;
                    foreach (Intent intent in inputComponent.Intents)
                    {
                        switch (intent.Intention)
                        {
                            case IntentEnum.MoveDown:
                            {
                                UIController.PickUpItemsScreen.ScrollSelectedTableDown();
                                break;
                            }
                            case IntentEnum.MoveUp:
                            {
                                UIController.PickUpItemsScreen.ScrollSelectedTableUp();
                                break;
                            }
                            case IntentEnum.ConetextualHotkeyPressed:
                                var selectedItem =
                                    UIController.PickUpItemsScreen.SelectedTable.Items.FirstOrDefault(x =>
                                        x.Hotkey == intent.PressedChar);

                                if (selectedItem != null)
                                {
                                    UIController.PickUpItemsScreen.SelectedTable.OnItemClick.Invoke(selectedItem);
                                }

                                break;
                            case IntentEnum.Enter:
                            {
                                UIController.PickUpItemsScreen.SelectedTable.OnItemClick.Invoke(UIController.PickUpItemsScreen
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
