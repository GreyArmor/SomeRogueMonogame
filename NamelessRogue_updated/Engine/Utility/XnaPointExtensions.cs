using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Utility
{
    public static class XnaPointExtensions
    {
        public static System.Drawing.Point ToPoint(this Point input)
        {
            return new System.Drawing.Point(input.X,input.Y);
        }
    }
}
