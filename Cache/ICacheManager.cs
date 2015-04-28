using System.Collections.Generic;
using CleanEmulatorFrontend.GamesData;

namespace CleanEmulatorFrontend.Cache
{
    public interface ICacheManager
    {
        void Write(IEnumerable<EmulatedSystem> emulatedSystems);
        CachedSystems Load();
    }
}