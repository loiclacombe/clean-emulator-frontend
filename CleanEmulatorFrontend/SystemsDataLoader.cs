using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AppConfig;
using Castle.Core.Internal;
using CleanEmulatorFrontend.Cache;
using CleanEmulatorFrontend.GamesData;
using CleanEmulatorFrontend.SqLiteCache;
using log4net;
using OtherParsers.Mame;
using MameLibrary = OtherParsers.Mame.Library;
using SplitSetLibrary = OtherParsers.SplitSet.Library;

namespace CleanEmulatorFrontend.GUI
{
    public class SystemsDataLoader
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (SystemsDataLoader));
        private readonly ICacheManager _cacheManager;
        private readonly Func<LoadedSystems> _cacheProvider;
        private readonly FromDatsGamesDataLoader _fromDatsGamesDataLoader;
        private readonly SystemConfigRootLoader _systemConfigRootLoader;
        private readonly UserConfiguration _userConfiguration;

        public SystemsDataLoader(Func<LoadedSystems> cacheProvider, SystemConfigRootLoader systemConfigRootLoader,
            ICacheManager cacheManager, FromDatsGamesDataLoader fromDatsGamesDataLoader)
        {
            _cacheProvider = cacheProvider;
            _systemConfigRootLoader = systemConfigRootLoader;
            _cacheManager = cacheManager;
            _fromDatsGamesDataLoader = fromDatsGamesDataLoader;
        }


        public async Task<LoadedSystems> LoadLibraries(bool forceReload = false)
        {
            var systemConfigRoot = await _systemConfigRootLoader.ReadEmuConfig();

            return await Task.Run(() =>
            {
                var rootGroups = systemConfigRoot.SystemNode;
                FillEmulatedSystems(systemConfigRoot, forceReload);
                systemConfigRoot.AllNodes
                    .Where(n => n.Items != null)
                    .ForEach(n => Array.Sort(n.Items,
                        (l, r) => l.Description.CompareTo(r.Description)));

                var loadedSystems = _cacheProvider();
                loadedSystems.Groups = rootGroups;
                return loadedSystems;
            });
        }

        private void FillEmulatedSystems(SystemConfigRoot systemConfigRoot, bool forceReload)
        {
            CachedSystems cachedData;

            if (forceReload)
            {
                cachedData = new CachedSystems();
                cachedData.Invalidate();
            }
            else
            {
                cachedData = _cacheManager.Load();
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (cachedData.IsValid)
            {
                ReadGamesFromCache(systemConfigRoot, cachedData);
            }
            else
            {
                ReadGamesFromDats(systemConfigRoot, cachedData);
            }

            stopwatch.Stop();
            Logger.DebugFormat("FillEmulatedSystems in {0} ", stopwatch.Elapsed);
            if (!cachedData.IsValid)
            {
                try
                {
                    _cacheManager.Write(systemConfigRoot.AllSystems);
                }
                catch (Exception e)
                {
                    Logger.Error("Could not write cache", e);
                    throw;
                }
            }
        }

        private void ReadGamesFromCache(SystemConfigRoot systemConfigRoot, CachedSystems cachedData)
        {
            systemConfigRoot.AllSystems.AsParallel()
                .ForEach(
                    es =>
                    {
                        Logger.DebugFormat("Reading system {0} from cache", es.Description);
                        es.Games = cachedData.EmulatedSystems[es.ShortName].Games;
                        PostProcessGames(es);
                    }
                );
        }


        private void ReadGamesFromDats(SystemConfigRoot systemConfigRoot, CachedSystems cachedData)
        {
            systemConfigRoot.AllSystems.AsParallel()
                .ForEach(
                    async es =>
                    {
                        Logger.DebugFormat("Reading system {0} from dats", es.Description);
                        var emulatedSystemSetsData = await (_fromDatsGamesDataLoader.LoadGamesData(systemConfigRoot,
                            es.CompatibleEmulator));
                        es.Games = emulatedSystemSetsData.Games;
                        PostProcessGames(es);
                    }
                );
        }

        private void PostProcessGames(EmulatedSystem emulatedSystem)
        {
            foreach (var game in emulatedSystem.Games)
            {
                game.System = emulatedSystem;
            }
        }

        private void FilterInvalidGames(EmulatedSystem emulatedSystem)
        {
            var validGames = emulatedSystem.Games.Where(
                IsValid).ToList();
            var invalidGamesCount = emulatedSystem.Games.Count() - validGames.Count();
            if (invalidGamesCount != 0)
            {
                Logger.WarnFormat("Filtered {0}", invalidGamesCount);
            }
            emulatedSystem.Games = validGames;
        }

        private static bool IsValid(Game g)
        {
            return g != null && g.Description != null && g.LaunchPath != null;
        }

        public async Task<LoadedSystems> ForceLoadFromDats()
        {
            _cacheManager.InvalidateCache();
            var forceLoadFromDats = await LoadLibraries(true);
            return forceLoadFromDats;
        }
    }
}