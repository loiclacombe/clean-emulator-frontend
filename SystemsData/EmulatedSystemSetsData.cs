using System.Collections.Generic;

namespace CleanEmulatorFrontend.GamesData
{
    public class EmulatedSystemSetsData
    {
        public EmulatedSystemSetsData()
        {
            Games = new List<Game>();
            LibraryMetadata = new Dictionary<string, string>();
        }

        public IList<Game> Games { get; set; }
        public IDictionary<string, string> LibraryMetadata { get; set; }
    }
}