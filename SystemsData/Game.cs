using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Xml.Serialization;

namespace GamesData
{
    public class Game
    {
        public readonly Guid Guid = Guid.NewGuid();
        public string Description { get; set; }
        public string BasePath { get; set; }
        public string LaunchPath { get; set; }

        public string AbsoluteLaunchPath
        {
            get
            {
                return BasePath != null ? Path.Combine(BasePath, LaunchPath) : LaunchPath;
            }
        }

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
