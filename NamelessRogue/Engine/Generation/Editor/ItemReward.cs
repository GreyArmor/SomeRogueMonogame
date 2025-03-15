using System.Xml.Serialization;

namespace NamelessRogue.Engine.Generation.Editor
{
    public partial class QuestTemplateData
    {
        [XmlRoot]
        public class ItemReward
        {
            public int amount;
            public FileReference rewardItem;

            [XmlElement]
            public int Amount { get => amount; set => amount = value; }

            [XmlElement]
            public FileReference RewardItem { get => rewardItem; set => rewardItem = value; }
        }
    }
}
