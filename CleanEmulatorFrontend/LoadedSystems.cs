using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using GamesData;

namespace CleanEmulatorFrontend
{
    public class LoadedSystems
    {
        public IEnumerable<SystemNode> Groups { get; set; }

        public IDictionary<string, Library> Libraries { get; set; }

        public IEnumerable<Game> FilterBy(EmulatedSystem emulatedSystem)
        {
            var games = EmptySet();
            emulatedSystem.Games.ForEach(g => games.Add(g));
            return games;
        }

        private static SortedSet<Game> EmptySet()
        {
            return new SortedSet<Game>(new GameComparer());
        }

        public IEnumerable<Game> FilterBy(SystemNode systemGroup)
        {
            var games = EmptySet();

            new List<SystemNode>{systemGroup}
                .Traverse(n=> n.Items)
                .OfType<SystemGroup>()
                .SelectMany(sg=> sg.EmulatedSystem)
                .ForEach(sg => sg.Games.ForEach(g => games.Add(g)));
            return games;
        }
    }
}