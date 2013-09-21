using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanEmulatorFrontend.Engine.Data;

namespace CleanEmulatorFrontend.Engine.Listers
{
    public interface GamesLister
    {
        List<Game> ListAll();
    }
}
