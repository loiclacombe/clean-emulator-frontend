using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using FluentAssertions;
using GamesData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parsers.SplitSet;
using Library = Parsers.SplitSet.Library;

namespace CleanEmulatorFrontend.Test
{
    /// <summary>
    /// Summary description for SplitSetTest
    /// </summary>
    [TestClass]
    public class SplitSetTest
    {
        private Library _library;

        public SplitSetTest()
        {
            _library = new Library();
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
            _library.Parse("SplitSet\\TestLibrary", result);
            var games = result.Games.ToList();
            games.Should().HaveCount(2);
            var firstGame = games[0];
            firstGame.Description.Should().Be("my other pretty rom (EN)");
            firstGame.LaunchPath.Should().EndWith("my other pretty rom (EN).zip");

            var secondGame = games[1];

            secondGame.Description.Should().Be("my pretty rom (EN)");
            secondGame.LaunchPath.Should().EndWith("my pretty rom (EN).zip");
        }
    }
}
