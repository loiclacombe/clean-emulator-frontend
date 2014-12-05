using System;
using System.Collections.Generic;
using Castle.Core.Internal;
using GamesData;
using GamesData.DatData;

namespace CleanEmulatorFrontend
{
    public class SystemsCache
    {
        public IEnumerable<SystemGroup> Groups { get; set; }

        public IEnumerable<GamesData.Game> FilterBySystem(EmulatedSystem emulatedSystem)
        {
            var games = EmptySet();
            emulatedSystem.Games.ForEach(g => games.Add(g));
            return games;
        }

        private static SortedSet<GamesData.Game> EmptySet()
        {
            return new SortedSet<GamesData.Game>(new GameComparer());
        }

        public IEnumerable<GamesData.Game> FilterBySystemGroup(SystemGroup systemGroup)
        {
            var games = EmptySet();
            systemGroup.EmulatedSystem.ForEach(sg => sg.Games.ForEach(g => games.Add(g)));
            return games;
        }
    }
}