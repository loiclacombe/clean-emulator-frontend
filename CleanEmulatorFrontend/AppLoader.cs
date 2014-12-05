using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Xml.Serialization;
using GamesData;
using Launchers;
using log4net;
using Parsers.Higan;
using Seterlund.CodeGuard;
using HiganLibrary = Parsers.Higan.Library;
using Library = GamesData.Library;

namespace CleanEmulatorFrontend
{
    public class AppLoader
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AppLoader));

        private readonly Func<SystemsCache> _cacheProvider;

        private Dictionary<string, Emulator> _emulators;
        private Dictionary<string, Library> _libraries;

        public AppLoader(Func<SystemsCache> cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public SystemsCache LoadLibraries()
        {
            SystemConfigRoot systemConfigRoot = ReadEmuConfig();

            FillEmulatorsDict(systemConfigRoot);
            FillLibrariesDict(systemConfigRoot);

            foreach (var emulatedSystem in systemConfigRoot.SystemGroup
                .Where(sg=> sg.Enabled)
                .SelectMany(systemGroup => systemGroup.EmulatedSystem))
            {
                ReadSystem(emulatedSystem);
            }

            SystemsCache cacheProvider = _cacheProvider();
            cacheProvider.Groups = systemConfigRoot.SystemGroup.Where(sg=> sg.Enabled);
            return cacheProvider;
        }

        private void FillLibrariesDict(SystemConfigRoot systemConfigRoot)
        {
            _libraries = systemConfigRoot.Library.ToDictionary(l => l.Name);
        }

        private void FillEmulatorsDict(SystemConfigRoot systemConfigRoot)
        {
            _emulators = systemConfigRoot.Emulator.ToDictionary(emulator => emulator.Name);
            foreach (var emulator in systemConfigRoot.Emulator)
            {
                emulator.Launcher = new GenericLauncher(emulator);
            }
        }

        private void ReadSystem(EmulatedSystem emulatedSystem)
        {

            emulatedSystem.Emulator = _emulators[emulatedSystem.CompatibleEmulator.Name];

            var libraryData = _libraries[emulatedSystem.CompatibleEmulator.LibraryName];
            Guard.That(libraryData).IsNotNull();

            string libraryClass = libraryData.LibraryClass;

            if (libraryClass == typeof (Parsers.SplitSet.Library).FullName)
            {
                var library = new Parsers.SplitSet.Library();
                library.Parse(libraryData.LibraryFolderKey, emulatedSystem);
            }
            else if (libraryClass == typeof(Parsers.Higan.Library).FullName)
            {
                var library = new HiganLibrary();
                library.Parse(libraryData.LibraryFolderKey, emulatedSystem);
            }
            else if (libraryClass == typeof (Parsers.Mame.Library).FullName)
            {
                var library = new Parsers.Mame.Library();
                library.Parse(emulatedSystem);
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