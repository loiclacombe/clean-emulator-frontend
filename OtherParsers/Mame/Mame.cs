using System;
using System.Xml.Serialization;

namespace OtherParsers.Mame
{
    [Serializable]
    [XmlRoot(IsNullable = false, ElementName = "mame")]
    public class Mame
    {
        /// <remarks />
        [XmlElement("game")]
        public Game[] Game { get; set; }
    }

    /// <remarks />
    public class Game
    {
        [XmlElement("description")]
        public string Description { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("year")]
        public string Year { get; set; }
    }
}