using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class InputSystem : ISystem
    {

      //  List<KeyEvent> pressedKeys;

        public InputSystem()
        {
         //   pressedKeys = new List<>();
        }

        long currentgmatime = 0;
        private long previousGametimeForMove = 0;

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            if (gameTime - previousGametimeForMove > 60)
            {
                previousGametimeForMove = gameTime;
                foreach (IEntity entity in namelessGame.GetEntities()) {
                    InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                    InputReceiver receiver = entity.GetComponentOfType<InputReceiver>();
                    if (receiver != null && inputComponent != null)
                    {
                            inputComponent.Intents.AddRange(KeyIntentTraslator.Translate(Keyboard.GetState().GetPressedKeys()));
                    }
                }
            }
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
