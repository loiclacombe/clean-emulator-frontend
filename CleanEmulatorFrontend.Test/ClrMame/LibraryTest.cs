using System.Linq;
using FluentAssertions;
using GamesData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parsers.ClrMame;
using Parser = Parsers.ClrMame.Parser;

namespace CleanEmulatorFrontend.Test.ClrMame
{
    [TestClass]
    public class LibraryTest
    {
        private readonly Library _library = new Library(new DatGrammar());

        [TestMethod]
        public void TestParse_clrMameHeader()
        {
            var emulatedSystem = new EmulatedSystem();
            _library.Parse(emulatedSystem,"ClrMame\\clrmame_header.dat");
            var libraryMetadata = emulatedSystem.LibraryMetadata;
            libraryMetadata.Should().Contain("name", "Nintendo - Super Nintendo Entertainment System");
            libraryMetadata.Should().Contain("description", "Nintendo - Super Nintendo Entertainment System");
            libraryMetadata.Should().Contain("version", "20141025-064847");
            libraryMetadata.Should().Contain("comment", "no-intro | www.no-intro.org");
        }

        [TestMethod]
        public void TestParse_verifiedGame()
        {
            var snesSystem = new EmulatedSystem();
             _library.Parse(snesSystem, "ClrMame\\verified_game.dat");
            var games = snesSystem.Games.ToList();
            games.Count.Should().Be(1);
            snesSystem.Description.Should().BeNull();
            var firstGame=games[0];
            firstGame.Description.Should().Be("ACME Animation Factory (USA)");
            firstGame.Roms.Count.Should().Be(1);
            firstGame.Roms[0].Name.Should().Be("ACME Animation Factory (USA).sfc");
        }

        [TestMethod]
        public void TestParse_fullDat()
        {
            var snesSystem = new EmulatedSystem();
             _library.Parse(snesSystem, "ClrMame\\snes.dat");
            snesSystem.Games.Count.Should().Be(3365);
        }
    }

}
