using AppConfig;
using CleanEmulatorFrontend.GamesData;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OtherParsers.Mame;
using Machine = OtherParsers.Mame.Machine;
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
            _library = new Library(new UserConfiguration());
        }

        [TestMethod]
        public void TestConvertMameGame()
        {
            var game = new Machine
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