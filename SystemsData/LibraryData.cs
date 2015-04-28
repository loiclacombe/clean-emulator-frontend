using System.Collections.Generic;

namespace CleanEmulatorFrontend.GamesData
{
    public class LibraryData
    {
        private IList<EmulatedSystem> _systems = new List<EmulatedSystem>();
        public IDictionary<string, string> Metadata { get; set; }

        public IList<EmulatedSystem> Systems
        {
            get { return _systems; }
            set { _systems = value; }
        }
    }
}