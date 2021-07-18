using System;
using NamelessRogue.shell;
using NamelessRogue.Engine.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NamelessRogue
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {           
          // SerializationCodeGenerator.GenerateStorages();
          // return;
            using (var game = new NamelessGame())
            {
                game.Run();
            }
        }
    }
#endif
}
