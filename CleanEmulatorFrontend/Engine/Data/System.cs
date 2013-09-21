using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanEmulatorFrontend.Engine.Listers;

namespace CleanEmulatorFrontend.Engine.Data
{
    public class System : IComparable<System>
    {
        private GamesLister _gamesLister;

        public System(GamesLister gamesLister)
        {
            _gamesLister = gamesLister;
        }

        public string Name {set;get;}

        public string ShortName { set; get; }

        public string Description {set;get;}



        public int CompareTo(System other)
        {
            return ShortName.CompareTo(other.ShortName);
        }
    }
}
