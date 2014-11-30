using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GamesData.DatData;

namespace GamesData
{
    public partial class EmulatedSystem
    {
        private IList<Game> _games=new List<Game>();

        [XmlIgnore]
        public Emulator Emulator { get; set; }
        [XmlIgnore]
        public IList<Game> Games {
            get { return _games; }
            set { _games = value; } 
        }
        [XmlIgnore]
        public IDictionary<string, string> LibraryMetadata { get; set; }
    }
}
