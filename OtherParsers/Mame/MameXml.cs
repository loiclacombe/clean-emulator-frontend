using System;
using System.Xml.Serialization;

namespace OtherParsers.Mame
{
    [Serializable]
    [XmlRoot(IsNullable = false, ElementName = "mame")]
    public class MameXml
    {
        /// <remarks />
        [XmlElement("machine")]
        public Machine[] Machine { get; set; }
    }

    /// <remarks />
    public class Machine
    {
        [XmlElement("description")]
        public string Description { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("year")]
        public string Year { get; set; }
    }
}