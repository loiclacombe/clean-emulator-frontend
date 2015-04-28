using System.Collections.Generic;
using System.Linq;
using CleanEmulatorFrontend.GamesData;

namespace CleanEmulatorFrontend.Cache
{
    public class CachedSystems
    {
        private bool _invalidated;

        public Dictionary<string,EmulatedSystem> EmulatedSystems { get; set; }

        public void Invalidate()
        {
            _invalidated = true;
        }

        public bool IsValid
        {
            get { return !_invalidated && EmulatedSystems.Any(); }
        }
    }
}