using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanEmulatorFrontend.Engine.Data;
using GamesData.DatData;

namespace CleanEmulatorFrontend.Engine.Listers
{
    public interface IGamesLister
    {
        IList<Game> ListAll();
    }
}
