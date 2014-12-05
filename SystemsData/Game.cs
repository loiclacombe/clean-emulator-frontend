using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using GamesData.DatData;

namespace GamesData
{
    public class Game
    {
        public readonly Guid Guid = Guid.NewGuid();
        public string Description { get; set; }
        public string LaunchPath { get; set; }
        public EmulatedSystem System { get; set; }
        public int CompareTo(Game other)
        {
            return Description.CompareTo(other.Description);
        }
    }

    public class GameComparer : IComparer<Game>
    {
        public int Compare(Game x, Game y)
        {
            return x.Description.CompareTo(y.Description);
        }
    }
}
