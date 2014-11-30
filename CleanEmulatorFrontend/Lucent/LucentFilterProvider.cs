using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesData;
using GamesData.DatData;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace CleanEmulatorFrontend.Lucent
{
    public class LucentFilterProvider
    {
        private readonly Func<Directory> _directoryProvider;
        public IDictionary<Guid, Game> GameDictionary;

        public LucentFilterProvider(Func<Directory> directoryProvider)
        {
            _directoryProvider = directoryProvider;
        }

        public IEnumerable<Game> LucentFilter(string search)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
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
    }
}
