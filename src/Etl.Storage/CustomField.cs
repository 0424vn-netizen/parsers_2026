using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Etl.Storage
{
    public class CustomField
    {
        [XmlAttribute]
        public bool Force { get; set; }

        [XmlAttribute]
        public string Text { get; set; }

        [XmlAttribute]
        public string Value { get; set; }

    }
}
