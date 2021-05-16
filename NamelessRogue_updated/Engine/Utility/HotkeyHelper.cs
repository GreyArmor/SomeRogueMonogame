using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace NamelessRogue.Engine.Utility
{
    public static class HotkeyHelper
    {
        public static string alphabet = "abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUWXYZ";
        public static char GetNextKey(char hotkey)
        {
            var foundIndex = alphabet.IndexOf(hotkey);
            if (foundIndex >= 0 && foundIndex!=alphabet.Length-1)
            {
                return alphabet[foundIndex + 1];
            }

            return char.MinValue;
        }
    }
}
