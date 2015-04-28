using CleanEmulatorFrontend.GamesData;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OtherParsers.Mame;
using Game = OtherParsers.Mame.Game;
using Library = OtherParsers.Mame.Library;

namespace CleanEmulatorFrontend.Test.Mame
{
    [TestClass]
    public class LibraryTest
    {
        private Library _library;

        [TestInitialize]
        public void Before()
        {
            _library = new Library();
        }

        [TestMethod]
        public void TestConvertMameGame()
        {
            var game = new Game
            {
                Description = "My little poney",
                Name = "myRomSetName"
            };
            var emulatedSystem = new EmulatedSystem();
            var result = game.AsGameDataOn();

            result.Description.Should().Be("My little poney");
            result.LaunchPath.Should().Be("myRomSetName");
        }
    }
}