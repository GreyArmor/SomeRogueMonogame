using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class MoveToCommand : Component {

        private IEntity entityToMove;

        public MoveToCommand(int x, int y, IEntity entityToMove) {
            this.entityToMove = entityToMove;
            p = new Point(x, y);
        }

        public MoveToCommand() {
            p = new Point();
        }

        public Point p;

        public IEntity getEntityToMove() {
            return entityToMove;
        }
    }
}
