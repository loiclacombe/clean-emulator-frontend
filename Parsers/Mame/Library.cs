using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GamesData;
using GamesData.DatData;

namespace Parsers.Mame
{
    public class Library : ILibrary
    {
        public LibraryData Parse(string datPath)
        {
            using (var stream = new FileStream(datPath, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(mame));
                var mameData = serializer.Deserialize(stream) as mame;
                Debug.Assert(mameData != null, "mameData != null");
                var games = mameData.game.Select(ConvertMameGame) ;

                var dat = new LibraryData ();
                return dat;
            }
        }

        public Game ConvertMameGame(game game)
        {
            return default(Game);
        }

        public LibraryData Parse()
        {
            throw new NotImplementedException();
        }

        public void Parse(EmulatedSystem emulatedSystem)
        {
            throw new NotImplementedException();
        }
    }
}
