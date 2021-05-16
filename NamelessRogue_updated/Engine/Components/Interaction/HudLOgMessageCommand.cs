using NamelessRogue_updated.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class HudLogMessageCommand : ICommand
    {
        public HudLogMessageCommand(string logMessage = "")
        {
            LogMessage = logMessage;
        }

        public string LogMessage { get; set; }
    }
}
