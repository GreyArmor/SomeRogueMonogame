using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NamelessRogue.Engine.Serialization
{
    [XmlRoot]
    public class ByteSizesStorage
    {
        [XmlArray]
        Dictionary<string, int> MaxSizesEvaluated { get; set; } = new Dictionary<string, int>();
    }
}
