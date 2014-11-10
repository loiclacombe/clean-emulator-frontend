using System.Collections.Generic;

namespace GamesData.DatData
{
    public class Dat
    {
        private IList<EmulatedSystem> _systems = new List<EmulatedSystem>();
        public IList<EmulatedSystem> Systems
        {
            get { return _systems; }
            set { _systems = value; }
        }

        public IDictionary<string, string> Metadata = new Dictionary<string, string>();
    }
}
