using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class TradeScreenIntentSystem : BaseSystem
    {
        public TradeScreenIntentSystem()
        {
            Signature = new HashSet<Type>();
        }
        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            InputComponent inputComponent = namelessGame.PlayerEntity.GetComponentOfType<InputComponent>();
            if (inputComponent != null && !inputComponent.IsDelayed)
            {
                foreach (Intent intent in inputComponent.Intents)
                {
                    switch (intent.Intention)
                    {
                        case IntentEnum.MoveUp:
                            {
                                UIContainer.Instance.TradeScreen.MoveCursorUp();
                                break;
                            }
                        case IntentEnum.MoveDown:
                            {
                                UIContainer.Instance.TradeScreen.MoveCursorDown();
                                break;
                            }
                        case IntentEnum.MoveRight:
                        case IntentEnum.MoveLeft:
                            {
                                if (UIContainer.Instance.TradeScreen.CursorMode == TradeCursorMode.LeftTable)
                                {
                                    UIContainer.Instance.TradeScreen.SwitchMode(TradeCursorMode.RightTable);
                                }
                                else if (UIContainer.Instance.TradeScreen.CursorMode == TradeCursorMode.RightTable)
                                {
                                    UIContainer.Instance.TradeScreen.SwitchMode(TradeCursorMode.LeftTable);
                                }

                            }
                            break;
                        case IntentEnum.Interact:
                            {
                                UIContainer.Instance.TradeScreen.SelectDeselectCurrentItem();
                                break;
                            }
                        case IntentEnum.Trade:
                            {

                            }
                            break;
                        case IntentEnum.Enter:
                            goto case IntentEnum.Interact;
                        case IntentEnum.Escape:
                            namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
                            break;
                        default:
                            break;
                    }
                }
                inputComponent.Intents.Clear();
            }
        }
    }
}
