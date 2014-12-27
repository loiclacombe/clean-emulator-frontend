using System.Linq;
using AppConfig;
using FluentAssertions;
using GamesData;
using GamesData.DatData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Library = OtherParsers.Higan.Library;

namespace CleanEmulatorFrontend.Test.Higan
{
    [TestClass]
    public class LibraryTest
    {
        private Library _library;
        private Mock<GamesData.Library> _xmlLibrary = new Mock<GamesData.Library>(MockBehavior.Strict);

        [TestInitialize]
        public void SetUp()
        {
            _library = new Library();
            _xmlLibrary.ResetCalls();
        }

        [TestMethod]
        public void TestParse()
        {
            var snesSystem = new EmulatedSystem();

            _xmlLibrary.SetupGet(x => x.Folder).Returns("Higan\\TestLibrary");

            _library.Parse(_xmlLibrary.Object, snesSystem);
            snesSystem.Games.Should().HaveCount(2);
            var games=snesSystem.Games.ToList();
            games[0].Description.Should().Be("Akumajou Dracula (Japan)");
            games[0].LaunchPath.Should().EndWith("Akumajou Dracula (Japan).sfc");
            games[1].Description.Should().Be("Final Fight 2 (NA) (1.0)");
            games[1].LaunchPath.Should().EndWith("Final Fight 2 (NA) (1.0).sfc");
            _xmlLibrary.Verify();
        }
    }
}