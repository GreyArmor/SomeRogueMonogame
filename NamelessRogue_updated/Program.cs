﻿using System;
using NamelessRogue.shell;
using NamelessRogue.Engine.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using RogueSharp.Random;
using NamelessRogue.Engine.Utility;
using NamelessRogue.Engine.Generation;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Components.Rendering;
using FlatSharp;
using NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses;
using NamelessRogue.Engine.Components.Physical;

namespace NamelessRogue
{
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
            //SerializationCodeGenerator.GenerateStorages(typeof(ConsoleCamera));
            //return;
            using (var game = new NamelessGame())
            {
                game.Run();
            }


        }    
    }
}