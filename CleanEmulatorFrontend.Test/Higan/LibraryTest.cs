using System.Linq;
using CleanEmulatorFrontend.GamesData;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CleanEmulatorFrontend.Test.Higan
{
    [TestClass]
    public class LibraryTest
    {
        private readonly Mock<Library> _xmlLibrary = new Mock<Library>(MockBehavior.Strict);
        private OtherParsers.Higan.Library _library;

        [TestInitialize]
        public void SetUp()
        {
            _library = new OtherParsers.Higan.Library();
            _xmlLibrary.ResetCalls();
        }

        [TestMethod]
        public void TestParse()
        {
            _xmlLibrary.SetupGet(x => x.Path).Returns("Higan\\TestLibrary");

            var snesSystem = _library.Parse(_xmlLibrary.Object);
            snesSystem.Games.Should().HaveCount(2);
            var games = snesSystem.Games.ToList();
            games[0].Description.Should().Be("Akumajou Dracula (Japan)");
            games[0].LaunchPath.Should().EndWith("Akumajou Dracula (Japan).sfc");
            games[1].Description.Should().Be("Final Fight 2 (NA) (1.0)");
            games[1].LaunchPath.Should().EndWith("Final Fight 2 (NA) (1.0).sfc");
            _xmlLibrary.Verify();
        }
    }
}