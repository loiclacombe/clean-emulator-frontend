using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GamesData
{
    public partial class Emulator
    {
        [XmlIgnore]
        public ILauncher Launcher { get; set; }
    }
}
