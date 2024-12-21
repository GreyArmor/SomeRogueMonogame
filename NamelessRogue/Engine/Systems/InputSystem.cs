using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Input;
using NamelessRogue.shell;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace NamelessRogue.Engine.Systems
{

    enum DelayState
    {
        FirstDelay,
        SecondDelay,
        ThirdDelay,
        NotDelayed,
            
    }
    public class InputSystem : BaseSystem
    {

        IKeyIntentTraslator translator;
        private readonly NamelessGame namelessGame;

        public InputSystem(IKeyIntentTraslator translator, NamelessGame namelessGame)
        {
            this.translator = translator;
            this.namelessGame = namelessGame;
            namelessGame.Window.TextInput += WindowOnTextInput;
            namelessGame.Window.KeyDown += Window_KeyDown;
            Signature.Add(typeof(InputComponent));
            Signature.Add(typeof(InputReceiver));
        }

        private void Window_KeyDown(object sender, InputKeyEventArgs e)
        {
            if (!namelessGame.CurrentContext.Systems.Contains(this))
            {
                return;
            }
        }

        long currentgmatime = 0;
        private long previousGametimeForMove = 0;

        private int firstDelayTime = 0;
        private int secondDelayTime = 400;
        private int thirdDelayTime = 200;
        private int noDelayTime = 30;
        DelayState delayState = DelayState.FirstDelay;

        int inputsTimeLimit = 30;

        private char lastCommand = Char.MinValue;
        private KeyboardState lastState;
        private Keys[] lastKeys = Array.Empty<Keys>();

        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {

            int delayTime = 0;
            switch (delayState)
            {
                case DelayState.FirstDelay:
                    delayTime = firstDelayTime;
                    break;
                case DelayState.SecondDelay:
                    delayTime = secondDelayTime;
                    break;
                case DelayState.ThirdDelay:
                    delayTime = thirdDelayTime;
                    break;
                default:
                    delayTime = noDelayTime;
                    break;
            }
            lastState = Keyboard.GetState();
            var newKeys = lastState.GetPressedKeys();
            var sameKeys = true;

            if (lastKeys.Length == newKeys.Length)
            {
                foreach (Keys key in newKeys)
                {
                    {
                        if (!lastKeys.Contains(key))
                        {
                            sameKeys = false;
                            break;
                        }
                    }
                }
            }

            if(!sameKeys)
            {
                delayState = DelayState.FirstDelay;
                delayTime = 0;
            }

            lastKeys = lastState.GetPressedKeys();

            InputComponent inputComponent = namelessGame.PlayerEntity.GetComponentOfType<InputComponent>();
            if (gameTime.TotalGameTime.TotalMilliseconds - previousGametimeForMove > delayTime)
            {
                inputComponent.IsDelayed = false;
                previousGametimeForMove = (long)gameTime.TotalGameTime.TotalMilliseconds;
           
                if (inputComponent != null)
                {
                    inputComponent.Intents.AddRange(translator.Translate(lastKeys, lastCommand, Mouse.GetState()));
                    lastCommand = Char.MinValue;
                }

                if (delayState != DelayState.NotDelayed)
                {
                    delayState++;
                }
            }
            else
            {
                inputComponent.IsDelayed = true;
            }
            if (lastState.GetPressedKeyCount() == 0)
            {
                delayState = DelayState.FirstDelay;
            }
          //  Debug.WriteLine(delayState);
            lastState = (default);

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
    }
}
