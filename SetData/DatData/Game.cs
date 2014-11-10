using System;
using System.Collections.Generic;

namespace GamesData.DatData
{
    public struct Game : IComparable<Game>
    {
        public string Description { get; set; }
        public string LaunchPath { get; set; }
        public IList<Rom> Roms { get; set; }
        public int CompareTo(Game other)
        {
            return this.Description.CompareTo(other.Description);
        }
    }
}
