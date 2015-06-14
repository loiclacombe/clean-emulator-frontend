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
        private readonly Func<GamesCacheEntities> _cacheEntitiesProvider;

        public SqLiteCacheManager(Func<GamesCacheEntities> cacheEntitiesProvider)
        {
            _cacheEntitiesProvider = cacheEntitiesProvider;
        }

        public void Write(IEnumerable<EmulatedSystem> emulatedSystems)
        {
            InvalidateCache();
            using (var cachedEntities = _cacheEntitiesProvider())
            using (var tx = cachedEntities.Database.BeginTransaction())
            {
                var insertedSystems = Persist(cachedEntities, emulatedSystems);
                CreateMissingBasePaths(cachedEntities, emulatedSystems);
                var dbBasePaths = cachedEntities.base_path
                    .Select(bp => new {bp.bp_path, bp.base_path_id})
                    .ToDictionary(bp => bp.bp_path, bp => bp.base_path_id);

                var joinedSystems = from cacheSystem in insertedSystems
                    join memSystem in emulatedSystems on cacheSystem.name equals memSystem.ShortName
                    select new {cachedSystem = cacheSystem, memSystem};

                var gamesToInsert = joinedSystems
                    .SelectMany(js => js.memSystem.Games
                        .Select(g => new
                        {
                            game = g,
                            js.cachedSystem,
                            basePathId = g.BasePath != null ? dbBasePaths[g.BasePath] : (long?) null
                        }));

                cachedEntities.games.AddRange(gamesToInsert.Select(g => new game
                {
                    description = g.game.Description,
                    emulated_system = g.cachedSystem,
                    launch_path = g.game.LaunchPath,
                    game_base_path = g.basePathId.HasValue
                        ? new game_base_path
                        {
                            base_path_id = g.basePathId
                        }
                        : null,
                    gguid = g.game.Guid
                }));
                cachedEntities.SaveChanges();

                tx.Commit();
            }
        }

        public CachedSystems Load()
        {
            var cachedSystems = new CachedSystems();

            try
            {
                using (var cachedEntities = _cacheEntitiesProvider())
                {
                    var emulatedSystems = cachedEntities.emulated_system.Select(es => new EmulatedSystem
                    {
                        ShortName = es.name
                    }
                        );

                    cachedEntities.games.Join(emulatedSystems, g => g.emulated_system.name, esbi => esbi.ShortName,
                        (g, esbi) => new Game
                        {
                            Guid = g.gguid,
                            BasePath = g.game_base_path.base_path.bp_path,
                            Description = g.description,
                            System = esbi
                        }).ToList().ForEach(g => { g.System.Games.Add(g); });

                    cachedSystems.EmulatedSystems = emulatedSystems.ToDictionary(es => es.ShortName);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed loading from cache", e);
                cachedSystems.Invalidate();
                InvalidateCache();
            }

            return cachedSystems;
        }

        public void InvalidateCache()
        {
            var tablesToPurge = new[] {"game_base_path", "last_played", "game", "emulated_system", "base_path"};

            var statements = tablesToPurge.Select(t => string.Format("DELETE FROM {0};", t));

            using (var cachedEntities = _cacheEntitiesProvider())
            using (var transaction = cachedEntities.Database.BeginTransaction())
            {
                var commands = string.Join(" ", statements);
                cachedEntities.Database.ExecuteSqlCommand(commands);
                transaction.Commit();
            }
        }

        private static IEnumerable<string> MemBasePaths(IEnumerable<EmulatedSystem> emulatedSystems)
        {
            return emulatedSystems.SelectMany(es => es.Games).Select(g => g.BasePath).Distinct();
        }

        private void CreateMissingBasePaths(GamesCacheEntities cachedEntities, IEnumerable<EmulatedSystem> emulatedSystems)
        {
            var memBasePaths = MemBasePaths(emulatedSystems);
            var inDbPaths = cachedEntities.base_path.Select(bp => bp.bp_path);
            var basePathToCreate = memBasePaths
                .Where(p => p != null)
                .Except(inDbPaths)
                .ToList();

            var inserted = cachedEntities.base_path.AddRange(basePathToCreate.Select(bp => new base_path
            {
                bp_path = bp
            }
                ));
            Logger.DebugFormat("Added {0} basepaths", inserted.Count());
            cachedEntities.SaveChanges();
        }

        private IEnumerable<emulated_system> Persist(GamesCacheEntities cachedEntities, IEnumerable<EmulatedSystem> emulatedSystems)
        {
            var entities = emulatedSystems
                .Select(es => es.ShortName)
                .ToList()
                .Select(shortName => new emulated_system
                {
                    name = shortName
                }).ToList();
            var insertedSystems = cachedEntities.emulated_system.AddRange(
                entities
                );
            return insertedSystems;
        }
    }
}