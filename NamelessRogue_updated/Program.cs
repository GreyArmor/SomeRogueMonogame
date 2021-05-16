using System;
using NamelessRogue.shell;
using NamelessRogue_updated.Engine.Serialization;

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

            SerializationCodeGenerator.Generate();

           // using (var game = new NamelessGame())
           //     game.Run();
        }
    }
#endif
}
