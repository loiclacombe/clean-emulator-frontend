using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Castle.Core.Internal;
using EmulatorModules;
using GamesData;
using GamesData.DatData;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Parsers.Higan;

namespace CleanEmulatorFrontend
{
    public class AppLoader
    {
        private readonly Func<SystemsCache> _cacheProvider;
        private readonly Library _higanLibrary;
        private readonly Func<IndexWriter> _indexWriterProviderFunc;

        public AppLoader(Library higanLibrary, Func<IndexWriter> indexWriterProviderFunc,
            Func<SystemsCache> cacheProvider)
        {
            _higanLibrary = higanLibrary;
            _indexWriterProviderFunc = indexWriterProviderFunc;
            _cacheProvider = cacheProvider;
        }


        public SystemsCache LoadLibraries()
        {
            SystemConfigRoot systemConfigRoot = ReadEmuConfig();

            Dictionary<string, Emulator> emulators = systemConfigRoot.Emulator.ToDictionary(emulator => emulator.Name);
            foreach (Emulator emulator in systemConfigRoot.Emulator)
            {
                emulator.Launcher = new GenericLauncher(emulator);
            }

            foreach (SystemGroup systemGroup in systemConfigRoot.SystemGroup)
            {
                foreach (EmulatedSystem emulatedSystem in systemGroup.EmulatedSystem)
                {
                    ReadSystem(emulatedSystem, emulators);
                }
            }


            SystemsCache cacheProvider = _cacheProvider();
            cacheProvider.Groups = systemConfigRoot.SystemGroup;
            return cacheProvider;
        }

        private void ReadSystem(EmulatedSystem emulatedSystem, Dictionary<string, Emulator> emulators)
        {
            emulatedSystem.Emulator = emulators[emulatedSystem.CompatibleEmulator.Name];
            string libraryClass = emulatedSystem.CompatibleEmulator.LibraryClass;

            if (libraryClass == typeof (Parsers.SplitSet.Library).FullName)
            {
                var library = new Parsers.SplitSet.Library();
                library.Parse(emulatedSystem);
            }
            else if (libraryClass == typeof (Library).FullName)
            {
                _higanLibrary.Parse(emulatedSystem);
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

        public void InitLucent(SystemsCache systemsCache, LibraryData data)
        {
            var dict = new Dictionary<Guid, Game>();
            IndexWriter writer = _indexWriterProviderFunc();
            data.Systems.ForEach(s => s.Games.ForEach(g =>
                                                      {
                                                          var document = new Document();
                                                          document.Add(new Field("system", s.Description, Field.Store.NO,
                                                              Field.Index.ANALYZED));
                                                          document.Add(new Field("game", g.Description, Field.Store.NO,
                                                              Field.Index.ANALYZED));
                                                          document.Add(new Field("guid", g.Guid.ToString(),
                                                              Field.Store.YES, Field.Index.NOT_ANALYZED));
                                                          writer.AddDocument(document);
                                                          dict.Add(g.Guid, g);
                                                      }
                ));

            writer.Optimize();
            writer.Dispose();
        }
    }
}