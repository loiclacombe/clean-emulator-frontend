using System;
using System.Collections.Generic;
using System.Linq;
using CleanEmulatorFrontend.Cache;
using CleanEmulatorFrontend.GamesData;
using log4net;

namespace CleanEmulatorFrontend.SqLiteCache
{
    public class SqLiteCacheManager : ICacheManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (SqLiteCacheManager));
        private readonly GamesCacheEntities _cacheEntities;

        public SqLiteCacheManager(GamesCacheEntities cacheEntities)
        {
            _cacheEntities = cacheEntities;
        }

        public void Write(IEnumerable<EmulatedSystem> emulatedSystems)
        {
            using (var tx = _cacheEntities.Database.BeginTransaction())
            {
                var insertedSystems = Persist(emulatedSystems);
                var memBasePaths = MemBasePaths(emulatedSystems);
                CreateMissingBasePaths(memBasePaths);
                var dbBasePaths = _cacheEntities.base_path.ToList().ToDictionary(bp => bp.bp_path, bp=> bp.base_path_id);

                var joinedSystems = from cacheSystem in insertedSystems
                    join memSystem in emulatedSystems on cacheSystem.name equals memSystem.ShortName
                    select new {cachedSystem = cacheSystem, memSystem};

                var gamesToInsert = joinedSystems
                    .SelectMany(js => js.memSystem.Games.Select(g => new {
                        game = g, 
                        js.cachedSystem,
                        basePathId = g.BasePath != null ? dbBasePaths[g.BasePath] : (long?)null
                    }));

                _cacheEntities.games.AddRange(gamesToInsert.Select(g => new game
                {
                    description = g.game.Description,
                    emulated_system = g.cachedSystem,
                    launch_path = g.game.LaunchPath,
                    game_base_path = g.basePathId.HasValue ? new game_base_path()
                    {
                        base_path_id = g.basePathId
                    }:null,
                    gguid = g.game.Guid,
                }));
                _cacheEntities.SaveChanges();

                tx.Commit();
            }
        }

        public CachedSystems Load()
        {
            var cachedSystems = new CachedSystems();

            var emulatedSystems =_cacheEntities.emulated_system.Select(es =>  new EmulatedSystem()
            {
                ShortName = es.name
            }
            );

            _cacheEntities.games.Join(emulatedSystems, g => g.emulated_system.name, esbi => esbi.ShortName,
                (g, esbi) => new Game()
                {
                    Guid = g.gguid,
                    BasePath = g.game_base_path.base_path.bp_path,
                    Description = g.description,
                    System = esbi
                }).ToList().ForEach(g =>
                {
                    g.System.Games.Add(g);
                });

            cachedSystems.EmulatedSystems = emulatedSystems.ToDictionary(es=> es.ShortName);

            return cachedSystems;
        }

        public void InvalidateCache()
        {
            _cacheEntities.Database.ExecuteSqlCommand("DELETE FROM game_base_path;DELETE FROM last_played;DELETE FROM game;DELETE FROM emulated_system;DELETE FROM base_path;");
        }

        private static IEnumerable<string> MemBasePaths(IEnumerable<EmulatedSystem> emulatedSystems)
        {
            return emulatedSystems.SelectMany(es => es.Games).Select(g => g.BasePath).Distinct();
        }

        private void CreateMissingBasePaths(IEnumerable<string> memBasePaths)
        {
            var inDbPaths = _cacheEntities.base_path.Select(bp => bp.bp_path).ToList();
            var basePathToCreate = memBasePaths
                .Where(p => p != null)
                .Except(inDbPaths)
                .ToList();

            var inserted = _cacheEntities.base_path.AddRange(basePathToCreate.Select(bp => new base_path
            {
                bp_path = bp
            }
                ));
            Logger.DebugFormat("Added {0} basepaths", inserted.Count());
            _cacheEntities.SaveChanges();
        }

        private IEnumerable<emulated_system> Persist(IEnumerable<EmulatedSystem> emulatedSystems)
        {
            var entities = emulatedSystems
                .Select(es => es.ShortName)
                .ToList()
                .Select(shortName => new emulated_system
                {
                    name = shortName
                }).ToList();
            var insertedSystems = _cacheEntities.emulated_system.AddRange(
                entities
                );
            return insertedSystems;
        }
    }
}