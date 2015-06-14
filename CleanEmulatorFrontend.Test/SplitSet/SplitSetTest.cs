using System.Linq;
using AppConfig;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Library = OtherParsers.SplitSet.Library;

namespace CleanEmulatorFrontend.Test
{
    /// <summary>
    ///     Summary description for SplitSetTest
    /// </summary>
    [TestClass]
    public class SplitSetTest
    {
        private readonly Library _library;
        private readonly Mock<GamesData.Library> _xmlLibrary = new Mock<GamesData.Library>(MockBehavior.Default);

        public SplitSetTest()
        {
            _library = new Library(new UserConfiguration());
            _xmlLibrary.ResetCalls();
            _xmlLibrary.CallBase = true;
        }

        [TestMethod]
        public void TestParse()
        {
            _xmlLibrary.Setup(ld => ld.Paths).Returns(new[]
            {
                "SplitSet\\TestLibrary"
            });
            _xmlLibrary.Object.RomExtension = new[] {".cue", ".zip"};
            var result = _library.Parse(_xmlLibrary.Object).Result;
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
    }
}