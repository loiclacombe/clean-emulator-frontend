using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using EnumerableUtils;

namespace CleanEmulatorFrontend.GamesData
{
    public partial class SystemConfigRoot
    {
        [XmlIgnore]
        public Dictionary<string, Library> LibrariesDict { get; private set; }

        [XmlIgnore]
        public Dictionary<string, Emulator> EmulatorsDict { get; private set; }

        public void InitializeDictionaries()
        {
            LibrariesDict = Library.ToDictionary(l => l.Name);
            EmulatorsDict = Emulator.ToDictionary(emulator => emulator.Name);
        }


        public IEnumerable<EmulatedSystem> AllSystems
        {
            get
            {
                var activeGroups = AllNodes
                    .OfType<EmulatedSystem>()
                    .Where(sg => sg.Enabled);
                return activeGroups;    
            }
        }

        public IEnumerable<SystemNode> AllNodes
        {
            get
            {
                return SystemNode.Traverse(
                    g => g.Items);
            }
        }
    }
}