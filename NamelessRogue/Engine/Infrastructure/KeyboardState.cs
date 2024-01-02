using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NamelessRogue.Engine.Infrastructure
{
    public struct KeyboardState
    {
        public List<Keys> Keys = new List<Keys>();
        public KeyboardState(){}
    }
}
