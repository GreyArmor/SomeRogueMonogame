using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Generation.World.Diplomacy
{
    public enum RelationsStatus
    {
        None,
        War,
        Peace,
        OpenBorders,
        Ally
    }

    public class RelationsModifier
    {
        public string Description;
        public int RaltionshipPoints;
    }
    public class PoliticalRelation
    {
        public List<RelationsModifier> Modifiers { get; set; } = new List<RelationsModifier>();
        public RelationsStatus Status { get; set; }

        public int RelationshipPoints { get; set; }
    }

}
