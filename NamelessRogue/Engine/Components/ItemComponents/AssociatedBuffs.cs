using System.Collections.Generic;
using System.Linq;

namespace NamelessRogue.Engine.Components.ItemComponents
{
    public class AssociatedBuffs : Component
    {
        public AssociatedBuffs(IEnumerable<string> buffIds)
        {
            BuffIds = buffIds.ToList();
        }

        public List<string> BuffIds { get; set; } = new List<string>();

        public override IComponent Clone()
        {
            var clone = new Consumable(BuffIds);
            return clone;
        }
    }
}
