using System.Linq;
using ClrMameParser;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Parsers.ClrMame;

namespace CleanEmulatorFrontend.Test.ClrMame
{
    [TestClass]
    public class LibraryTest
    {
        private readonly Library _library = new Library(new DatGrammar());
        private readonly Mock<GamesData.Library> _libraryData = new Mock<GamesData.Library>();

        public void Initialize()
        {
            _libraryData.ResetCalls();
        }

        [TestMethod]
        public void TestParse_clrMameHeader()
        {
            _libraryData.SetupGet(ld => ld.Path).Returns("ClrMame\\clrmame_header.dat");
            var emulatedSystem = _library.Parse(_libraryData.Object);
            var libraryMetadata = emulatedSystem.LibraryMetadata;
            libraryMetadata.Should().Contain("name", "Nintendo - Super Nintendo Entertainment System");
            libraryMetadata.Should().Contain("description", "Nintendo - Super Nintendo Entertainment System");
            libraryMetadata.Should().Contain("version", "20141025-064847");
            libraryMetadata.Should().Contain("comment", "no-intro | www.no-intro.org");
        }

        [TestMethod]
        public void TestParse_verifiedGame()
        {
            _libraryData.SetupGet(ld => ld.Path).Returns("ClrMame\\verified_game.dat");
            var snesSystem = _library.Parse(_libraryData.Object);
            var games = snesSystem.Games.ToList();
            games.Count.Should().Be(1);
            var firstGame = games[0];
            firstGame.Description.Should().Be("ACME Animation Factory (USA)");
            firstGame.LaunchPath.Should().Be("ACME Animation Factory (USA)");
        }

        [TestMethod]
        public void TestParse_fullDat()
        {
            _libraryData.SetupGet(ld => ld.Path).Returns("ClrMame\\snes.dat");
            var snesSystem = _library.Parse(_libraryData.Object);
            snesSystem.Games.Count.Should().Be(3365);
        }
    }
}