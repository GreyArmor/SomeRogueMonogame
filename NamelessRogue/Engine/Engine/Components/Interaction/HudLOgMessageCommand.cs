using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class HudLogMessageCommand : Component
    {
        public HudLogMessageCommand(string logMessage = "")
        {
            LogMessage = logMessage;
        }

        public string LogMessage { get; set; }
    }
}
