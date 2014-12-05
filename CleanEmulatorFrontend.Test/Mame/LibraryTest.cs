using System;
using AppConfig;
using FluentAssertions;
using GamesData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parsers.Mame;
using Game = Parsers.Mame.Game;
using Library = Parsers.Mame.Library;

namespace CleanEmulatorFrontend.Test.Mame
{
    [TestClass]
    public class LibraryTest
    {
        private Library _library;

        [TestInitialize]
        public void Before()
        {
            _library=new Library();
        }

        [TestMethod]
        public void TestConvertMameGame()
        {
            var game = new Game()
                       {
                           Description ="My little poney",
                           Name = "myRomSetName"
                       };
            var emulatedSystem = new EmulatedSystem();
            var result=game.AsGameDataOn(emulatedSystem);

            result.Description.Should().Be("My little poney");
            result.LaunchPath.Should().Be("myRomSetName");
            result.System.Should().BeSameAs(emulatedSystem);
        }
    }
}
