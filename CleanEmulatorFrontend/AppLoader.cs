using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Castle.Core.Internal;
using GamesData;
using Launchers;
using log4net;
using Parsers;
using Seterlund.CodeGuard;
using HiganLibrary = Parsers.Higan.Library;

namespace CleanEmulatorFrontend
{
    public class AppLoader
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof (AppLoader));

        private readonly Func<SystemsCache> _cacheProvider;
        private readonly Dictionary<string, ILibrary> _librariesParser;

        private Dictionary<string, Emulator> _emulators;
        private Dictionary<string, Library> _libraries;

        public AppLoader(Func<SystemsCache> cacheProvider)
        {
            _cacheProvider = cacheProvider;
            _librariesParser = new Dictionary<string, ILibrary>();


            _librariesParser.Add(typeof (Parsers.SplitSet.Library).FullName,
                new Parsers.SplitSet.Library());
            _librariesParser.Add(typeof (HiganLibrary).FullName,
                new HiganLibrary());
            _librariesParser.Add(typeof (Parsers.Mame.Library).FullName,
                new Parsers.Mame.Library());
        }

        public SystemsCache LoadLibraries()
        {
            SystemConfigRoot systemConfigRoot = ReadEmuConfig();

            FillEmulatorsDict(systemConfigRoot);
            FillLibrariesDict(systemConfigRoot);

            foreach (EmulatedSystem emulatedSystem in systemConfigRoot.SystemGroup
                .Where(sg => sg.Enabled)
                .SelectMany(systemGroup => systemGroup.EmulatedSystem))
            {
                ReadSystem(emulatedSystem);
                FilterInvalidGames(emulatedSystem);
            }

            SystemsCache cacheProvider = _cacheProvider();
            cacheProvider.Groups = systemConfigRoot.SystemGroup.Where(sg => sg.Enabled);
            return cacheProvider;
        }

        private void FilterInvalidGames(EmulatedSystem emulatedSystem)
        {
            var filteredList = emulatedSystem.Games.Where(
                IsInvalid).ToList();
            int invalidGamesCount = emulatedSystem.Games.Count() - filteredList.Count();
            if (invalidGamesCount != 0)
            {
                _logger.WarnFormat("Filtered {0}", invalidGamesCount);
            }
            emulatedSystem.Games = filteredList;
        }

        private static bool IsInvalid(Game g)
        {
            return g.Description != null && g.LaunchPath != null && g.System != null;
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

            string libraryFolder = ConfigurationManager.AppSettings[libraryData.LibraryFolderKey];
            ILibrary library = _librariesParser[libraryData.LibraryClass];

            if (library != null)
            {
                library.Parse(libraryFolder, emulatedSystem);
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

