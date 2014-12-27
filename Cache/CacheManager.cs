using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamesData;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cache
{
    public class CacheManager
    {
        private const string CachePath = @"%APPDATA%\CleanEmulatorFrontend\cache.json";
        private static readonly ILog Logger = LogManager.GetLogger(typeof (CacheManager));

        private static string ExpandCachePath
        {
            get { return Environment.ExpandEnvironmentVariables(CachePath); }
        }

        public void Write(IEnumerable<SystemGroup> systemGroups)
        {
            JsonSerializer serializer = JsonSerializer();

            var cacheContainer = new CacheContainer {SystemGroups = systemGroups.ToList()};
            using (var sw = new StreamWriter(ExpandCachePath))
            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, cacheContainer);
            }
        }

        private static JsonSerializer JsonSerializer()
        {
            var serializer = new JsonSerializer
                             {
                                 NullValueHandling = NullValueHandling.Ignore,
                                 ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                                 PreserveReferencesHandling = PreserveReferencesHandling.All
                             };
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            return serializer;
        }


        public List<SystemGroup> Load()
        {
            try
            {
                JsonSerializer serializer = JsonSerializer();
                using (var sw = new StreamReader(ExpandCachePath))
                using (var reader = new JsonTextReader(sw))
                {
                    return serializer.Deserialize<CacheContainer>(reader).SystemGroups;
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e, e);
                return new List<SystemGroup>();
            }
        }
    }
}