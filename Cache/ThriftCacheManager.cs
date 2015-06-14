using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CleanEmulatorFrontend.Cache.Thrift.Data;
using log4net;
using Thrift.Protocol;
using Thrift.Transport;
using CEmulatedSystem = CleanEmulatorFrontend.Cache.Thrift.Data.EmulatedSystem;
using CGame = CleanEmulatorFrontend.Cache.Data.Game;
using GDGame = CleanEmulatorFrontend.GamesData.Game;
using GDEmulatedSystem = CleanEmulatorFrontend.GamesData.EmulatedSystem;


namespace CleanEmulatorFrontend.Cache.Thrift
{
    public class ThriftCacheManager : ICacheManager
    {
        private const string CachePath = @"%APPDATA%\CleanEmulatorFrontend\cache.bin";
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ThriftCacheManager));

        public ThriftCacheManager()
        {
        }

        public virtual string ExpandedCachePath
        {
            get { return Environment.ExpandEnvironmentVariables(CachePath); }
        }

        public void Write(IEnumerable<GDEmulatedSystem> emulatedSystems)
        {
            Logger.InfoFormat("Writing cache to {0}", ExpandedCachePath);
            var persisted = new PersistedEmulatedSystems
            {
                EmulatedSystemsData = emulatedSystems
                    .Select(es => new CEmulatedSystem
                    {
                        ShortName = es.ShortName,
                        Games = es.Games.Select(g=> new CGame
                        {
                            BasePath = g.BasePath,
                            LaunchPath = g.LaunchPath,
                            Description = g.Description
                        }).ToList()
                    }).ToList()
            };
            using (var stream = new FileStream(ExpandedCachePath, FileMode.Create))
            using (TProtocol tProtocol = new TBinaryProtocol(new TStreamTransport(Stream.Null, stream)))
            {
                persisted.Write(tProtocol);
            }
        }

        public CachedSystems Load()
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Logger.InfoFormat("Reading cache from {0}", ExpandedCachePath);
                using (var stream = new FileStream(ExpandedCachePath, FileMode.Open))
                using (TProtocol tProtocol = new TBinaryProtocol(new TStreamTransport(stream, Stream.Null)))
                {
                    var emulatedSystems = new PersistedEmulatedSystems();
                    emulatedSystems.Read(tProtocol);
                    var cachedSystems = new CachedSystems
                    {
                        EmulatedSystems = emulatedSystems.EmulatedSystemsData.Select(es => new GDEmulatedSystem
                        {
                            ShortName = es.ShortName,
                            Games = es.Games.Select(g => new GDGame
                            {
                                BasePath = g.BasePath,
                                LaunchPath = g.LaunchPath,
                                Description = g.Description
                            }).ToList()
                        }).ToDictionary(es=> es.ShortName)
                    };
                    stopwatch.Stop();
                    Logger.DebugFormat("Read cache in {0}", stopwatch.Elapsed);
                    return cachedSystems;
                }
            }
            catch (Exception e)
            {
                Logger.Debug("Cache loading failed.", e);
                return new CachedSystems
                {
                    EmulatedSystems = new Dictionary<string, GDEmulatedSystem>()
                };
            }
        }

        public void InvalidateCache()
        {
            try
            {
                Logger.Info("Invalidated cache");
                new FileInfo(ExpandedCachePath).Delete();
            }
            catch (Exception e)
            {
                Logger.Debug("Could not delete cache", e);
            }
        }
    }
}