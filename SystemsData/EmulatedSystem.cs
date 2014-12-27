using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace GamesData
{
    public partial class EmulatedSystem
    {
        private IList<Game> _games=new List<Game>();
        private IList<string> _extensions = new List<string>();

        [XmlIgnore]
        [JsonIgnore]
        public Emulator Emulator { get; set; }

        [XmlIgnore]
        public IList<Game> Games
        {
            get { return _games; }
            set { _games = value; }
        }
        [XmlIgnore]
        public IDictionary<string, string> LibraryMetadata { get; set; }
        [XmlIgnore]
        public IList<string> Extensions
        {
            get { return _extensions; }
            set { _extensions = value; }
        }
    }
}
