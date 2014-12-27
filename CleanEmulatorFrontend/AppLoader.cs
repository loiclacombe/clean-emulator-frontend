using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cache;
using GamesData;
using Launchers;
using log4net;
using Parsers;
using Seterlund.CodeGuard;
using HiganLibrary = OtherParsers.Higan.Library;
using MameLibrary = OtherParsers.Mame.Library;
using SplitSetLibrary = OtherParsers.SplitSet.Library;

namespace CleanEmulatorFrontend
{
    public class AppLoader
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (AppLoader));
        private readonly CacheManager _cacheManager;

        private readonly Func<LoadedSystems> _cacheProvider;
        private readonly Dictionary<string, ILibrary> _librariesParser;

        private Dictionary<string, Emulator> _emulators;
        private Dictionary<string, Library> _libraries;

        public AppLoader(Func<LoadedSystems> cacheProvider, CacheManager cacheManager,
            SplitSetLibrary splitSetLibrary,
            HiganLibrary higanLibrary, MameLibrary mameLibrary)
        {
            _cacheProvider = cacheProvider;
            _librariesParser = new Dictionary<string, ILibrary>();
            _cacheManager = cacheManager;
            _librariesParser.Add(typeof (OtherParsers.SplitSet.Library).FullName,
                splitSetLibrary);
            _librariesParser.Add(typeof (HiganLibrary).FullName, higanLibrary);
            _librariesParser.Add(typeof (OtherParsers.Mame.Library).FullName,
                mameLibrary);
        }

        public LoadedSystems LoadLibrariesFromDats()
        {
            LoadedSystems provider = _cacheProvider();
            SystemConfigRoot systemConfigRoot = ReadEmuConfig();

            FillEmulatorsDict(systemConfigRoot);
            FillLibrariesDict(systemConfigRoot);

            var activeGroups = systemConfigRoot.SystemGroup
                .Where(sg => sg.Enabled);
            foreach (EmulatedSystem emulatedSystem in activeGroups
                .SelectMany(systemGroup => systemGroup.EmulatedSystem))
            {
                ReadSystem(emulatedSystem);
                FilterInvalidGames(emulatedSystem);
            }

            var systemGroups = activeGroups.ToList();
            systemGroups.ForEach(sg => Array.Sort(sg.EmulatedSystem, 
                (l, r)=> l.Description.CompareTo(r.Description)));

            provider.Groups = systemGroups;
            _cacheManager.Write(provider.Groups.ToList());

            return provider;
        }

        public LoadedSystems LoadLibrariesFromCache()
        {
            LoadedSystems provider = _cacheProvider();
            List<SystemGroup> systemGroups = _cacheManager.Load();

            if (systemGroups.Any())
            {
                provider.Groups = systemGroups;
            }
            else
            {
                provider = LoadLibrariesFromDats();
            }

            return provider;
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

        private SystemConfigRoot ReadEmuConfig()
        {
            Assembly assembly = Assembly.GetAssembly(typeof (SystemConfigRoot));
            using (Stream stream = assembly.GetManifestResourceStream("GamesData.emuconfig.xml"))
            {
                var serializer = new XmlSerializer(typeof (SystemConfigRoot));
                var systemConfigRoot = serializer.Deserialize(stream) as SystemConfigRoot;
                return systemConfigRoot;
            }
        }
    }
}