using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CleanEmulatorFrontend.Cache;
using CleanEmulatorFrontend.GamesData;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SDEmulatedSystem = CleanEmulatorFrontend.GamesData.EmulatedSystem;

namespace CacheTest
{
    [TestClass]
    public class ThriftCacheManagerTest
    {
        [TestMethod]
        public void SaveAndLoad()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            var cachePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            var managerMock = new Mock<ThriftCacheManager>();
            var cacheManager = managerMock.Object;
            var game = new Game
            {
                Description = "Super Mario Sunshine",
                LaunchPath = "/sms",
                BasePath = "/toto"
            };
            var emulatedSystem = new SDEmulatedSystem
            {
                Description = "TitiSystem",
                ShortName = "TS",
                Games = new[]
                {
                    game
                }
            };
            var emulatedSystems = new List<SDEmulatedSystem> {emulatedSystem};

            managerMock.Setup(m => m.ExpandedCachePath)
                .Returns(cachePath);

            cacheManager.Write(emulatedSystems);
            var systems = cacheManager.Load();
            var resultSystem = systems.EmulatedSystems.FirstOrDefault();
            resultSystem.Key.Should().Be(emulatedSystem.ShortName);
            resultSystem.Value.Should().BeNull();
            resultSystem.Value.ShortName.Should().Be(emulatedSystem.ShortName);
            resultSystem.Value.Games.Count.Should().Be(1);
            resultSystem.Value.Games[0].Should().Be(game);
        }
    }
}