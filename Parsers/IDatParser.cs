using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesData.DatData;

namespace Parsers
{
    public interface IDatParser
    {
        Dat Parse(string datfolder);
    }
}
