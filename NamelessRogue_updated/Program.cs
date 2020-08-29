using System;
using NamelessRogue.shell;

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
            using (var game = new NamelessGame())
                game.Run();
        }
    }
#endif
}
