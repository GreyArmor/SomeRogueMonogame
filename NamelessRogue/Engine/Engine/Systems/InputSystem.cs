using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class InputSystem : BaseSystem
    {

        IKeyIntentTraslator translator;
        private readonly NamelessGame namelessGame;

        public InputSystem(IKeyIntentTraslator translator, NamelessGame namelessGame)
        {
            this.translator = translator;
            this.namelessGame = namelessGame;
            namelessGame.Window.TextInput += WindowOnTextInput;

            Signature.Add(typeof(InputComponent));
            Signature.Add(typeof(InputReceiver));
        }

        long currentgmatime = 0;
        private long previousGametimeForMove = 0;

        private char lastCommand = Char.MinValue;
        private KeyboardState lastState;

        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(long gameTime, NamelessGame namelessGame)
        {
            if (gameTime - previousGametimeForMove > 90)
            {
                previousGametimeForMove = gameTime;
                foreach (IEntity entity in RegisteredEntities) {
                    InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                    InputReceiver receiver = entity.GetComponentOfType<InputReceiver>();
                    if (receiver != null && inputComponent != null && lastState != default)
                    {
                        inputComponent.Intents.AddRange(translator.Translate(lastState.GetPressedKeys(), lastCommand));
                        lastCommand = Char.MinValue;
                        lastState = default;
                    }
                }
            }

        }

        private void WindowOnTextInput(object sender, TextInputEventArgs e)
        {
            if (!namelessGame.CurrentContext.Systems.Contains(this))
            {
                return;
            }

            lastCommand = e.Character;
            lastState = Keyboard.GetState();
        }

        //public void keyPressed(KeyEvent e)
        //{
        //    pressedKeys = new List<>();
        //    pressedKeys.Add(e);
        //}

        //public void keyReleased(KeyEvent e)
        //{
        //    Optional<KeyEvent> key = pressedKeys.stream().filter(x => x.getKeyCode() == e.getKeyCode()).findFirst();
        //    if (key.isPresent())
        //    {
        //        pressedKeys.Remove(key.get());
        //    }
        //}

    }
}
