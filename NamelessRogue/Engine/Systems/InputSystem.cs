using System;
using System.Collections.Generic;
using System.Linq;
using Veldrid;

using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Input;
using NamelessRogue.shell;
using NamelessRogue.Engine.Infrastructure;
using KeyboardState = NamelessRogue.Engine.Infrastructure.KeyboardState;

namespace NamelessRogue.Engine.Systems
{
    public class InputSystem : BaseSystem
    {

        IKeyIntentTraslator translator;
        private readonly NamelessGame game;

        public InputSystem(IKeyIntentTraslator translator, NamelessGame game)
        {
            this.translator = translator;
            this.game = game;

            Signature.Add(typeof(InputComponent));
            Signature.Add(typeof(InputReceiver));
        }

		long currentgmatime = 0;
        private long previousGametimeForMove = 0;

        int inputsTimeLimit = 0;

        private char lastCommand = Char.MinValue;

        public override HashSet<Type> Signature { get; } = new HashSet<Type>();
        MouseState mouseState;
        KeyboardState keyboardState;
		public override void Update(GameTime gameTime, NamelessGame game)
        {

            mouseState = new MouseState(game.Input);
            keyboardState = new KeyboardState(game.Input);

            if (keyboardState.Key.Any())
            {
                foreach (IEntity entity in RegisteredEntities)
                {
                    InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                    inputComponent.Intents.AddRange(translator.Translate(keyboardState.Key.ToArray(), lastCommand, mouseState));
                }
                lastCommand = Char.MinValue;
            }

            //	if (gameTime.TotalGameTime.TotalMilliseconds - previousGametimeForMove > inputsTimeLimit)
            {
                previousGametimeForMove = (long)gameTime.TotalGameTime.TotalMilliseconds;
                foreach (IEntity entity in RegisteredEntities)
                {
                    InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                    InputReceiver receiver = entity.GetComponentOfType<InputReceiver>();
                    if (receiver != null && inputComponent != null)
                    {
                        inputComponent.Intents.AddRange(translator.Translate(keyboardState.Key.ToArray(), lastCommand, mouseState));
                        lastCommand = Char.MinValue;
                    }
                }
            }
        }
    }
}
