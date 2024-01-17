using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Veldrid;


namespace NamelessRogue.Engine.Input
{
    public class MainMenuKeyIntentTranslator : IKeyIntentTraslator
    {
        public List<Intent> Translate(Key[] keyCodes, char lastCommand, MouseState mouseState)
        {
            return new List<Intent>();
        }
    }
}
