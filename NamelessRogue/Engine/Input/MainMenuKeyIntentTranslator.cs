﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NamelessRogue.Engine.Input
{
    public class MainMenuKeyIntentTranslator : IKeyIntentTraslator
    {
        public List<Intent> Translate(Keys[] keyCodes, char lastCommand, MouseState mouseState)
        {
            return new List<Intent>();
        }
    }
}
