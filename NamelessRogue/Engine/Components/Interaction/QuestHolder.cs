using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Generation.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NamelessRogue.Engine.Components.Interaction
{
    public enum QuestStatus
    {
        InProgress, 
        Finished,
        Failed
    }
    public class QuestHolder : Component
    {
        public List<Quest> Quests { get; set; }
        public QuestHolder(){}
    }

    public class Quest
    {
        public QuestStatus Status { get; set; }
        public List<string> Milestones { get; set; }
        public QuestNode RooNode { get; set; }
        public QuestNode CurrentNode { get; set; }

    }

    public class QuestNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public QuestType ProgressCondition { get; set; }
        public List<QuestNode> ChildNodes { get; set; }
        public List<IEntity> Items { get; set; }
        public List<IEntity> NPCs { get; set; }
        public List<IEntity> Locations { get; set; }
    }

}
