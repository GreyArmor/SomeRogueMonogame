using NamelessRogue.Engine.Components.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static NamelessRogue.Engine.Generation.Editor.QuestTemplateData;

namespace NamelessRogue.Engine.Generation.Editor
{
    [XmlRoot]
    public partial class QuestTemplateData
    {
        public string id = "";
        public string name = "";
        public string filename = "";
        public string description = "";

        [XmlElement]
        public string Id { get => id; set => id = value; }
        [XmlElement]
        public string FileName { get => filename; set => filename = value; }
        [XmlElement]
        public string Name { get => name; set => name = value; }
        [XmlElement]
        public string Description { get => description; set => description = value; }
        [XmlArray]
        public List<QuestBranch> QuestBranches { get; set; } = new List<QuestBranch>();
    }

    [XmlRoot]
    public class QuestBranch
    {
        public string id = Guid.NewGuid().ToString();
        public string name = "";
        public string description = "";
        public int moneyReward = 0;

        [XmlElement]
        public string Id { get => id; set => id = value; }
        [XmlElement]
        public string Name { get => name; set => name = value; }
        [XmlElement]
        public string Description { get => description; set => description = value; }

        public int QuestTypeIndex = 0;
        private QuestType questType = QuestType.GetFromLocation;
        [XmlElement]
        public QuestType QuestType { get => questType; set => questType = value; }

        [XmlElement]
        public QuestObjectiveData Objective { get; set; }

        [XmlElement]
        public int MoneyReward { get => moneyReward; set => moneyReward = value; }

        [XmlArray]
        public List<ItemReward> ItemReward { get; set; } = new List<ItemReward>();

        [XmlElement]
        public FileReference NextQuestInChain { get; set; }

    }
}
