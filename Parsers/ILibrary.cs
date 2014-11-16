using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesData;
using GamesData.DatData;

namespace Parsers
{
    public interface ILibrary
    {

        void Parse(EmulatedSystem emulatedSystem);
    }
}
