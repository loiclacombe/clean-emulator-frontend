using System;
using System.Collections.Generic;

namespace GamesData.DatData
{
    public class Game : IComparable<Game>
    {
        public readonly Guid Guid = Guid.NewGuid();
        public string Description { get; set; }
        public string LaunchPath { get; set; }
        public IList<Rom> Roms { get; set; }
        public EmulatedSystem System { get; set; }
        public int CompareTo(Game other)
        {
            return this.Description.CompareTo(other.Description);
        }
    }
}
