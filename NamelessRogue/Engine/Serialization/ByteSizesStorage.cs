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
        public void Add(string typeString, int byteSize)
        {
            Keys.Add(typeString);
            Values.Add(byteSize);
        }
        [XmlArray]
        public List<string> Keys { get; set; } = new List<string>();

        [XmlArray]
        public List<int> Values { get; set; } = new List<int>();
    }
}
