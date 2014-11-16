using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Xml.Serialization;
using AppConfig;
using Castle.Core.Internal;
using EmulatorModules;
using GamesData;
using GamesData.DatData;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Parsers;
using ClrMame = Parsers.ClrMame;
using SplitSet = Parsers.SplitSet;
using EmulatedSystem = GamesData.EmulatedSystem;
using Higan = Parsers.Higan;

namespace CleanEmulatorFrontend
{
    public class AppLoader
    {
        private readonly Func<IndexWriter> _indexWriterProviderFunc;
        private readonly Func<SystemsCache> _cacheProvider;
        private Higan.Library _higanLibrary;

        public AppLoader(Higan.Library higanLibrary, Func<IndexWriter> indexWriterProviderFunc,
            Func<SystemsCache> cacheProvider)
        {
            _higanLibrary = higanLibrary;
            _indexWriterProviderFunc = indexWriterProviderFunc;
            _cacheProvider = cacheProvider;
        }


        public SystemsCache LoadLibraries()
        {
            SystemConfigRoot systemConfigRoot=ReadEmuConfig();

            var emulators= systemConfigRoot.Emulator.ToDictionary(emulator => emulator.Name);
            foreach (var emulator in systemConfigRoot.Emulator)
            {
                emulator.Launcher=new GenericLauncher(emulator);
            }

            foreach (var systemGroup in systemConfigRoot.SystemGroup)
            {
                foreach (var emulatedSystem in systemGroup.EmulatedSystem)
                {
                    emulatedSystem.Emulator = emulators[emulatedSystem.CompatibleEmulator.Name];
                    var libraryClass = emulatedSystem.CompatibleEmulator.LibraryClass;

                    if (libraryClass == typeof(SplitSet.Library).FullName)
                    {
                        var library=new SplitSet.Library();
                        library.Parse(emulatedSystem);
                    }
                    else if (libraryClass == typeof(Higan.Library).FullName)
                    {
                        _higanLibrary.Parse(emulatedSystem);
                    }

                }
            }


            var cacheProvider = _cacheProvider();
            cacheProvider.Groups = systemConfigRoot.SystemGroup;
            return cacheProvider;
        }

        private SystemConfigRoot ReadEmuConfig()
        {

            var assembly = Assembly.GetAssembly(typeof(SystemConfigRoot));
            using (var stream = assembly.GetManifestResourceStream("GamesData.emuconfig.xml"))
            {
                var serializer = new XmlSerializer(typeof (SystemConfigRoot));
                var systemConfigRoot = serializer.Deserialize(stream) as SystemConfigRoot;
                return systemConfigRoot;
            }
        }

        public void InitLucent(SystemsCache systemsCache, LibraryData data)
        {
            systemsCache.GameDictionary = new Dictionary<Guid, Game>();
            var writer = _indexWriterProviderFunc();
            data.Systems.ForEach(s => s.Games.ForEach(g =>
            {
                var document = new Document();
                document.Add(new Field("system", s.Description, Field.Store.NO, Field.Index.ANALYZED));
                document.Add(new Field("game", g.Description, Field.Store.NO, Field.Index.ANALYZED));
                document.Add(new Field("guid", g.Guid.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                writer.AddDocument(document);
                systemsCache.GameDictionary.Add(g.Guid, g);
            }
                ));

            writer.Optimize();
            writer.Dispose();
        }

    }
}