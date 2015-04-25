using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cache;
using GamesData;
using log4net;
using log4net.Repository.Hierarchy;
using Launchers;
using Parsers;
using Seterlund.CodeGuard;
using HiganLibrary = OtherParsers.Higan.Library;
using MameLibrary = OtherParsers.Mame.Library;
using SplitSetLibrary = OtherParsers.SplitSet.Library;

namespace CleanEmulatorFrontend
{
    public class DatsLoader
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DatsLoader));

        private readonly Func<LoadedSystems> _cacheProvider;

        private readonly Func<SystemConfigRoot> _systemConfigRootProvider;
        private readonly CacheLoader _cacheLoader;
        private readonly Dictionary<string, ILibrary> _librariesParser;
        private Dictionary<string, Library> _libraries;
        private Dictionary<string, Emulator> _emulators;

        public DatsLoader(Func<LoadedSystems> cacheProvider, Func<SystemConfigRoot> systemConfigRootProvider,
            CacheLoader cacheLoader, OtherParsers.SplitSet.Library splitSetLibrary,
            HiganLibrary higanLibrary, OtherParsers.Mame.Library mameLibrary)
        {
            _cacheProvider = cacheProvider;
            _systemConfigRootProvider = systemConfigRootProvider;
            _cacheLoader = cacheLoader;

            _librariesParser = new Dictionary<string, ILibrary>
            {
                {typeof (SplitSetLibrary).FullName, splitSetLibrary},
                {typeof (HiganLibrary).FullName, higanLibrary},
                {typeof (MameLibrary).FullName, mameLibrary}
            };

        }

        public LoadedSystems LoadLibraries()
        {
            LoadedSystems loadedSystems = _cacheProvider();
            SystemConfigRoot systemConfigRoot = _systemConfigRootProvider();

            FillEmulatorsDict(systemConfigRoot);
            FillLibrariesDict(systemConfigRoot);

            var rootGroups = systemConfigRoot.SystemNode
                .Where(sg => sg.Enabled);
            var activeGroups = rootGroups.Traverse(
                g => g.Items)
                .OfType<SystemGroup>()
                .Where(sg => sg.EmulatedSystem != null && sg.Enabled);

            var emulatedSystems = activeGroups
                .Where(sg => sg.EmulatedSystem != null)
                .SelectMany(systemGroup => systemGroup.EmulatedSystem);
            foreach (EmulatedSystem emulatedSystem in emulatedSystems)
            {
                ReadSystem(emulatedSystem);
                FilterInvalidGames(emulatedSystem);
            }

            var systemGroups = activeGroups.ToList();
            systemGroups.ForEach(sg => Array.Sort(sg.EmulatedSystem,
                (l, r) => l.Description.CompareTo(r.Description)));

            loadedSystems.Groups = rootGroups;
            _cacheLoader.Write(loadedSystems.Groups.ToList());

            return loadedSystems;
        }


        private void FilterInvalidGames(EmulatedSystem emulatedSystem)
        {
            List<Game> validGames = emulatedSystem.Games.Where(
                IsValid).ToList();
            int invalidGamesCount = emulatedSystem.Games.Count() - validGames.Count();
            if (invalidGamesCount != 0)
            {
                Logger.WarnFormat("Filtered {0}", invalidGamesCount);
            }
            emulatedSystem.Games = validGames;
        }

        private static bool IsValid(Game g)
        {
            return g != null && g.Description != null && g.LaunchPath != null && g.System != null;
        }

        private void FillLibrariesDict(SystemConfigRoot systemConfigRoot)
        {
            _libraries = systemConfigRoot.Library.ToDictionary(l => l.Name);
        }

        private void FillEmulatorsDict(SystemConfigRoot systemConfigRoot)
        {
            _emulators = systemConfigRoot.Emulator.ToDictionary(emulator => emulator.Name);
            foreach (Emulator emulator in systemConfigRoot.Emulator)
            {
                emulator.Launcher = new GenericLauncher(emulator);
            }
        }

        private void ReadSystem(EmulatedSystem emulatedSystem)
        {
            emulatedSystem.Emulator = _emulators[emulatedSystem.CompatibleEmulator.Name];

            Library libraryData = _libraries[emulatedSystem.CompatibleEmulator.LibraryName];
            Guard.That(libraryData).IsNotNull();

            ILibrary library = _librariesParser[libraryData.LibraryClass];

            if (library != null)
            {
                library.Parse(libraryData, emulatedSystem);
            }
        }
    }
}
