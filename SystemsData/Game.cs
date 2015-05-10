using System;
using System.Collections.Generic;
using System.IO;

namespace CleanEmulatorFrontend.GamesData
{
    public class Game
    {
        public Guid Guid { get; set; }
        public string Description { get; set; }
        public string BasePath { get; set; }
        public string LaunchPath { get; set; }
        public EmulatedSystem System { get; set; }

        public Game()
        {
            Guid = Guid.NewGuid();
        }

        public string AbsoluteLaunchPath
        {
            get { return BasePath != null ? Path.Combine(BasePath, LaunchPath) : LaunchPath; }
        }

        public int CompareTo(Game other)
        {
            return Description.CompareTo(other.Description);
        }

        protected bool Equals(Game other)
        {
            return Equals(System, other.System) && string.Equals(LaunchPath, other.LaunchPath) && string.Equals(BasePath, other.BasePath) && string.Equals(Description, other.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Game) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (System != null ? System.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LaunchPath != null ? LaunchPath.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (BasePath != null ? BasePath.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
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