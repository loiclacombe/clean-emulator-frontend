using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using GamesData;
using GamesData.DatData;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace CleanEmulatorFrontend
{
    public class SystemsCache
    {
        public IDictionary<Guid, Game> GameDictionary;
        public IEnumerable<SystemGroup> Groups { get; set; }
        private Func<Directory> _directoryProvider;

        public SystemsCache(Func<Directory> directoryProvider)
        {
            _directoryProvider = directoryProvider;
        }

        public IEnumerable<Game> FilterBySystem(EmulatedSystem emulatedSystem)
        {
            var games =new SortedSet<Game>();
            emulatedSystem.Games.ForEach(g => games.Add(g));
            return games;
        }

        public IEnumerable<Game> LucentFilter(string search)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            var searcher = new IndexSearcher(_directoryProvider());
            var parser = new QueryParser(Version.LUCENE_30, "game", analyzer);

            Query query = parser.Parse(search);
            TopDocs hits = searcher.Search(query, 5000);
            var foundGames = new SortedSet<Game>();
            for (int i = 0; i < hits.TotalHits; i++)
            {
                // get the document from index
                var doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                var guid = Guid.Parse(doc.Get("guid"));
                var game = GameDictionary[guid];
                foundGames.Add(game);
            }

            return foundGames;
        }

        public IEnumerable<Game> FilterBySystemGroup(SystemGroup systemGroup)
        {
            var games = new SortedSet<Game>();
            systemGroup.EmulatedSystem.ForEach(sg => sg.Games.ForEach(g => games.Add(g)));
            return games;
        }
    }
}
