using System;
using System.Collections.Generic;

namespace GamesData.DatData
{
    public class EmulatedSystem : IComparable<EmulatedSystem>
    {
        private IList<Game> _games=new List<Game>();

        public IList<Game> Games
        {
            get { return _games; }
            set { _games = value; }
        }

        public string ShortName { set; get; }

        public string Description {set;get;}
        public string Manufacturer { set; get; }

        public int CompareTo(EmulatedSystem other)
        {
            return ShortName.CompareTo(other.ShortName);
        }
    }
}
