using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using CleanEmulatorFrontend.Cache;
using CleanEmulatorFrontend.GamesData;
using log4net;
using HiganLibrary = OtherParsers.Higan.Library;
using MameLibrary = OtherParsers.Mame.Library;
using SplitSetLibrary = OtherParsers.SplitSet.Library;

namespace CleanEmulatorFrontend.GUI
{
    public class SystemsDataLoader
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (SystemsDataLoader));
        private readonly Func<LoadedSystems> _cacheProvider;
        private readonly FromDatsGamesDataLoader _fromDatsGamesDataLoader;
        private readonly SystemConfigRootLoader _systemConfigRootLoader;
        private readonly ThriftCacheManager _thriftCacheManager;

        public SystemsDataLoader(Func<LoadedSystems> cacheProvider, SystemConfigRootLoader systemConfigRootLoader,
            ThriftCacheManager thriftCacheManager, FromDatsGamesDataLoader fromDatsGamesDataLoader)
        {
            _cacheProvider = cacheProvider;
            _systemConfigRootLoader = systemConfigRootLoader;
            _thriftCacheManager = thriftCacheManager;
            _fromDatsGamesDataLoader = fromDatsGamesDataLoader;
        }

        public async Task<LoadedSystems> LoadLibraries()
        {
            var systemConfigRoot = await _systemConfigRootLoader.ReadEmuConfig();
            return await Task.Run(() =>
            {
                var rootGroups = systemConfigRoot.SystemNode;
                FillEmulatedSystems(systemConfigRoot);
                systemConfigRoot.AllNodes
                    .Where(n => n.Items != null)
                    .ForEach(n => Array.Sort(n.Items,
                        (l, r) => l.Description.CompareTo(r.Description)));

                var loadedSystems = _cacheProvider();
                loadedSystems.Groups = rootGroups;
                return loadedSystems;
            });
        }

        private void FillEmulatedSystems(SystemConfigRoot systemConfigRoot)
        {
            var emulatedSystems = systemConfigRoot.AllSystems;

            var cachedData = _thriftCacheManager.Load();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var emulatedSystem in emulatedSystems)
            {
                emulatedSystem.Emulator = systemConfigRoot.EmulatorsDict[emulatedSystem.CompatibleEmulator.Name];
                if (cachedData.IsValid && cachedData.EmulatedSystems.ContainsKey(emulatedSystem.ShortName))
                {
                    Logger.DebugFormat("Reading system {0} from cache", emulatedSystem.Description);
                    emulatedSystem.Games = cachedData.EmulatedSystems[emulatedSystem.ShortName].Games;
                }
                else
                {
                    cachedData.Invalidate();
                    Logger.DebugFormat("Reading system {0} from dats", emulatedSystem.Description);
                    var emulatedSystemSetsData = _fromDatsGamesDataLoader.LoadGamesData(systemConfigRoot,
                        emulatedSystem.CompatibleEmulator);
                    emulatedSystem.Games = emulatedSystemSetsData.Games;
                }

                FilterInvalidGames(emulatedSystem);
                PostProcessGames(emulatedSystem);
            }
            stopwatch.Stop();
            Logger.DebugFormat("FillEmulatedSystems in {0} ", stopwatch.Elapsed);
            if (!cachedData.IsValid)
            {
                _thriftCacheManager.Write(emulatedSystems.ToList());
            }
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
            _thriftCacheManager.InvalidateCache();
            var forceLoadFromDats = await LoadLibraries();
            return forceLoadFromDats;
        }
    }
}