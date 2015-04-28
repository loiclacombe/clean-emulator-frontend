using System.Xml.Serialization;

namespace CleanEmulatorFrontend.GamesData
{
    public partial class Emulator
    {
        [XmlIgnore]
        public ILauncher Launcher { get; set; }
    }
}