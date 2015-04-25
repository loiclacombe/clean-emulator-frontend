using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using FluentAssertions;
using GamesData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Library = OtherParsers.SplitSet.Library;

namespace CleanEmulatorFrontend.Test
{
    /// <summary>
    /// Summary description for SplitSetTest
    /// </summary>
    [TestClass]
    public class SplitSetTest
    {
        private Library _library;
        private Mock<GamesData.Library> _xmlLibrary = new Mock<GamesData.Library>(MockBehavior.Strict);

        public SplitSetTest()
        {
            _library = new Library();
            _xmlLibrary.ResetCalls();
            _xmlLibrary.CallBase = true;
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestParse()
        {
            var result = new EmulatedSystem();
            _xmlLibrary.SetupGet(ld => ld.Path).Returns("SplitSet\\TestLibrary");
            _xmlLibrary.Object.RomExtension = new[] { "cue" };
            _library.Parse(_xmlLibrary.Object, result);
            var games = result.Games.ToList();
            games.Sort((l, r) => l.Description.CompareTo(r.Description));
            games.Should().HaveCount(3);
            var firstGame = games[0];
            firstGame.Description.Should().Be("my game iso");
            firstGame.LaunchPath.Should().EndWith("my game iso.cue");
            
            var secondGame = games[1];

            secondGame.Description.Should().Be("my other pretty rom (EN)");
            secondGame.LaunchPath.Should().EndWith("my other pretty rom (EN).zip");

            var thirdGame = games[2];

            thirdGame.Description.Should().Be("my pretty rom (EN)");
            thirdGame.LaunchPath.Should().EndWith("my pretty rom (EN).zip");
        }
    }
}
