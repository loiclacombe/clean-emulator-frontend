using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanEmulatorFrontend.Engine.Data;
using GamesData.DatData;
using Parsers;

namespace CleanEmulatorFrontend.Engine.Listers
{
    public class GenericGameLister : IGamesLister
    {
        private readonly IDatParser _datParser;
        private readonly string _filename;

        public GenericGameLister(IDatParser datParser, string filename)
        {
            _datParser = datParser;
            _filename = filename;
        }

        public IList<Game> ListAll()
        {
            return null;
        }
    }
}
