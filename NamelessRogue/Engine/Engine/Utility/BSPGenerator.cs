using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Engine.Utility
{
    public class BSPTree
    {
        public BSPTree(Rectangle bounds)
        {
            Bounds = bounds;
        }

        public void Split(Random random,int minimumSize)
        {
            bool vertical = random.Next(2) > 0;
            if (vertical)
            {
                var newX = random.Next(Bounds.X+ minimumSize, Bounds.X + Bounds.Width);
                ChildA = new BSPTree(new Rectangle(Bounds.X,Bounds.Y,newX - Bounds.X,Bounds.Height));
                ChildB = new BSPTree(new Rectangle(newX, Bounds.Y, Bounds.Width - (newX - Bounds.X), Bounds.Height));
            }
            else
            {
                var newY = random.Next(Bounds.Y+ minimumSize, Bounds.Y + Bounds.Height);
                ChildA = new BSPTree(new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, newY - Bounds.Y));
                ChildB = new BSPTree(new Rectangle(Bounds.X, newY, Bounds.Width, Bounds.Height - (newY - Bounds.Y)));
            }
        }

        public Rectangle Bounds;
        public BSPTree ChildA;
        public BSPTree ChildB;

    }
}
