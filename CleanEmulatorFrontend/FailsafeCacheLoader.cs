using System;
using System.Collections.Generic;
using System.Linq;
using Cache;
using GamesData;

namespace CleanEmulatorFrontend
{
    public class FailsafeCacheLoader
    {
        private readonly Func<LoadedSystems> _cacheProvider;
        private readonly CacheLoader _cacheLoader;
        private readonly DatsLoader _datsLoader;

        public FailsafeCacheLoader(Func<LoadedSystems> cacheProvider, CacheLoader cacheLoader, DatsLoader datsLoader)
        {
            _datsLoader = datsLoader;
            _cacheProvider = cacheProvider;
            _cacheLoader = cacheLoader;
        }

        public LoadedSystems LoadLibraries()
        {
            LoadedSystems provider = _cacheProvider();
            List<SystemNode> systemGroups = _cacheLoader.Load();

            if (systemGroups.Any())
            {
                provider.Groups = systemGroups;
            }
            else
            {
                provider = _datsLoader.LoadLibraries();
            }

            return provider;
        }

        public LoadedSystems ForceLoadFromDats()
        {
            return _datsLoader.LoadLibraries();
        }
    }
}