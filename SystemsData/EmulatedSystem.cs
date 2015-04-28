using System.Collections.Generic;
using System.Xml.Serialization;

namespace CleanEmulatorFrontend.GamesData
{
    public partial class EmulatedSystem
    {
        private IList<string> _extensions = new List<string>();
        private IList<Game> _games = new List<Game>();

        [XmlIgnore]
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