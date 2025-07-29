using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.Systems.Inventory;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems
{
    internal class AbilityScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            var playerEntity = namelessGame.PlayerEntity;
            var inputComponent = namelessGame.InputEntity.GetComponentOfType<InputComponent>();
            foreach (Intent intent in inputComponent.Intents)
            {
                switch (intent.Intention)
                {
                    case IntentEnum.MoveUp:
                    case IntentEnum.MoveDown:
                    case IntentEnum.MoveLeft:
                    case IntentEnum.MoveRight:
                    case IntentEnum.MoveTopLeft:
                    case IntentEnum.MoveTopRight:
                    case IntentEnum.MoveBottomLeft:
                    case IntentEnum.MoveBottomRight:
                    case IntentEnum.MoveAscent:
                    case IntentEnum.MoveDescent:
                        break;

                    case IntentEnum.Interact:
                        break;
                    case IntentEnum.Escape:
                        BackToGame(namelessGame);
                        break;
                    case IntentEnum.Add:
                        break;
                    case IntentEnum.Substract:
                        break;
                    default:
                        break;
                }
            }
            inputComponent.Intents.Clear();
        }

        internal void BackToGame(NamelessGame game)
        {
            game.ContextToSwitch = ContextFactory.GetIngameContext(game);
        }

    }
}
