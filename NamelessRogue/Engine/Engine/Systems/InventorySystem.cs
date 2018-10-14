using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class InventorySystem : ISystem
    {

        public void Update(long gameTime, NamelessGame namelessGame)
        { 
            foreach (IEntity entity in namelessGame.GetEntities()) {
                ItemsHolder itemsHolder = entity.GetComponentOfType<ItemsHolder>();
                InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                if (itemsHolder != null && inputComponent != null)
                {
                    foreach (Intent intent in inputComponent.Intents) {
                        switch (intent)
                        {
                            case Intent.DropItem:
                            {
//                            Item item = EntityManager.GetComponent(inputComponent.Target);
//                            item.setHolderId(null);
//                            itemsHolder.getItems().Remove(item);
                            }
                                break;
                            case Intent.PlaceItem:
                            {
//                            Item item = EntityManager.GetComponent(inputComponent.Target);
//                            item.setHolderId(itemsHolder.getId());
//                            itemsHolder.getItems().Add(item);
                            }
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
        }
    }
}