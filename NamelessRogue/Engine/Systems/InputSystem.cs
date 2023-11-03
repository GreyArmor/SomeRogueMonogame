using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems
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
            lastState = Keyboard.GetState();
        }

		long currentgmatime = 0;
        private long previousGametimeForMove = 0;

        int inputsTimeLimit = 0;

        private char lastCommand = Char.MinValue;
        private KeyboardState lastState;

        public override HashSet<Type> Signature { get; } = new HashSet<Type>();
		List<Keys> immediatePressedKeys = new List<Keys>();
        MouseState LastMouseState { get; set; }
        MouseState MouseState { get; set; }
        bool mouseStateChanged = false;
		public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            //should probably move to monogame keys anyway, if im not doing roguelike controls anymore
			var keyPressed = Keyboard.GetState();
			if (keyPressed.IsKeyDown(Keys.W)) immediatePressedKeys.Add(Keys.W);
			if (keyPressed.IsKeyDown(Keys.S)) immediatePressedKeys.Add(Keys.S);
			if (keyPressed.IsKeyDown(Keys.A)) immediatePressedKeys.Add(Keys.A);
			if (keyPressed.IsKeyDown(Keys.D)) immediatePressedKeys.Add(Keys.D);

			MouseState = Mouse.GetState();

            if (MouseState != LastMouseState)
            {
                mouseStateChanged = true;
			}

			if (immediatePressedKeys.Any() || mouseStateChanged)
            {
                foreach (IEntity entity in RegisteredEntities)
                {
					InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                    //InputReceiver receiver = entity.GetComponentOfType<InputReceiver>();
                    inputComponent.Intents.AddRange(translator.Translate(immediatePressedKeys.ToArray(), lastCommand, MouseState));
                }
                immediatePressedKeys.Clear();
				lastCommand = Char.MinValue;
				lastState = default;
                LastMouseState = MouseState;
                mouseStateChanged = false;
			}
			if (gameTime.TotalGameTime.TotalMilliseconds - previousGametimeForMove > inputsTimeLimit)
            {
                previousGametimeForMove = (long)gameTime.TotalGameTime.TotalMilliseconds;
                foreach (IEntity entity in RegisteredEntities) {
                    InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                    InputReceiver receiver = entity.GetComponentOfType<InputReceiver>();
                    if (receiver != null && inputComponent != null && lastState != default)
                    {
                        inputComponent.Intents.AddRange(translator.Translate(lastState.GetPressedKeys(), lastCommand, MouseState));
                        lastCommand = Char.MinValue;
                        lastState = default;
						LastMouseState = MouseState;
						mouseStateChanged = false;
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
