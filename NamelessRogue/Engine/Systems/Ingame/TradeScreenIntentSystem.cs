using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Stats;
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
            InputComponent inputComponent = namelessGame.InputEntity.GetComponentOfType<InputComponent>();
            var mode = UIContainer.Instance.TradeScreen.CursorMode;
            if (inputComponent != null && !inputComponent.IsDelayed)
            {
                foreach (Intent intent in inputComponent.Intents)
                {
                    switch (intent.Intention)
                    {
                        case IntentEnum.MoveUp:
                            {
                                switch (mode)
                                {
                                    case TradeCursorMode.RightTable:
                                    case TradeCursorMode.LeftTable:
                                        UIContainer.Instance.TradeScreen.MoveCursorUp();
                                        break;
                                    case TradeCursorMode.RightTableFilters:
                                    case TradeCursorMode.LeftTableFilters:
                                        UIContainer.Instance.TradeScreen.MoveCursorUp();
                                        break;
                                }

                                break;
                            }
                        case IntentEnum.MoveDown:
                            {
                                switch (mode)
                                {
                                    case TradeCursorMode.RightTable:
                                    case TradeCursorMode.LeftTable:
                                        UIContainer.Instance.TradeScreen.MoveCursorDown();
                                        break;
                                    case TradeCursorMode.RightTableFilters:
                                    case TradeCursorMode.LeftTableFilters:
                                        UIContainer.Instance.TradeScreen.MoveFilterCursorDown();
                                        break;
                                }
                                break;
                            }
                        case IntentEnum.MoveRight:
                        case IntentEnum.MoveLeft:
                            {                               
                                switch (mode)
                                {
                                    case TradeCursorMode.RightTable:
                                        UIContainer.Instance.TradeScreen.SwitchMode(TradeCursorMode.LeftTable);
                                        break;
                                    case TradeCursorMode.LeftTable:
                                        UIContainer.Instance.TradeScreen.SwitchMode(TradeCursorMode.RightTable);
                                        break;
                                    case TradeCursorMode.RightTableFilters:
                                    case TradeCursorMode.LeftTableFilters:
                                        switch(intent.Intention)
                                        {
                                            case IntentEnum.MoveRight:
                                                UIContainer.Instance.TradeScreen.MoveFilterCursorRight();
                                                break;
                                            case IntentEnum.MoveLeft:
                                                UIContainer.Instance.TradeScreen.MoveFilterCursorLeft();
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
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
                                var total = UIContainer.Instance.TradeScreen.GetTotal();
                                var rightTableMoney = UIContainer.Instance.TradeScreen.GetRightTableMoney();
                                var leftTableMoney = UIContainer.Instance.TradeScreen.GetLeftTableMoney();
                                var unableToTrade = UIContainer.Instance.TradeScreen.IsUnableToTrade(total, leftTableMoney, rightTableMoney);
                                UIContainer.Instance.TradeScreen.CreateTrade(total);
                            }
                            break;
                        case IntentEnum.SwitchTarget:
                            {
                                switch (mode)
                                {
                                    case TradeCursorMode.LeftTable:
                                        UIContainer.Instance.TradeScreen.SwitchMode(TradeCursorMode.LeftTableFilters);
                                        break;
                                    case TradeCursorMode.LeftTableFilters:
                                        UIContainer.Instance.TradeScreen.SwitchMode(TradeCursorMode.RightTableFilters);
                                        break;
                                    case TradeCursorMode.RightTable:
                                        UIContainer.Instance.TradeScreen.SwitchMode(TradeCursorMode.LeftTable);
                                        break;
                            
                                    case TradeCursorMode.RightTableFilters:
                                        UIContainer.Instance.TradeScreen.SwitchMode(TradeCursorMode.RightTable);
                                        break;
                                   
                                }

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
