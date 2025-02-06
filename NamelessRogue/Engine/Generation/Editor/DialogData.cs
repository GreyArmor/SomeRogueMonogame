using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NamelessRogue.Engine.Generation.Editor
{

    public enum DialogOutcomeId
    {
        None,
        CancelDialog,
        NPCBecomesHostile,
        NPCBecomesNeutral,
        NPCBecomesFriendly,
        NPCStartTrade,
        NPCBribeTrade,
        NPCAuthorizeAccess,
        MissionStart,
        MissionProgress,
        MissionEnd,
    }

    [XmlRoot]
    public class DialogData
    {
        [XmlElement]
        public string Id { get; set; }
        [XmlElement]
        public string Response { get; set; }
        [XmlArray]
        public DialogOption[] Options { get; set; }
        
        public DialogData() {}
    }
    public class DialogOption
    {
        [XmlElement]
        public string Id { get; set; }
        [XmlElement]
        public string OptionText { get; set; }
        [XmlElement]
        public string DialogOutcomeData { get; set; }

        [XmlElement]
        public DialogOutcomeId DialogOutcomeId { get; set; }

        [XmlElement]
        public bool HasDialogOutcomeData { get; set; }
        [XmlElement]
        public  DialogData DialogData{ get; set; }
         

    }
}
