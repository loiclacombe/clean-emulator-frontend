using System;
using System.Collections.Generic;
using System.Linq;
using AppConfig;
using Castle.Core.Internal;
using CleanEmulatorFrontend.GamesData;
using EnumerableUtils;
using Library = CleanEmulatorFrontend.GamesData.Library;

namespace CleanEmulatorFrontend.GUI
{
    public class LoadedSystems
    {
        public IDictionary<Guid, GameStats> GameStats = new Dictionary<Guid, GameStats>();
        public IEnumerable<SystemNode> Groups { get; set; }
        public IDictionary<Guid, Library> Libraries { get; set; }

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

            new List<SystemNode> {systemGroup}
                .Traverse(n => n.Items)
                .OfType<EmulatedSystem>()
                .ForEach(sg => sg.Games.ForEach(g => games.Add(g)));
            return games;
        }

        public void RecordGameLaunch(Game game)
        {
            GameStats curGameStats;
            if (GameStats.ContainsKey(game.Guid))
            {
                curGameStats = GameStats[game.Guid];
            }
            else
            {
                curGameStats=new GameStats();
                GameStats[game.Guid] = curGameStats;
            }
            curGameStats.IncrementLaunchCounter();
        }
    }
}