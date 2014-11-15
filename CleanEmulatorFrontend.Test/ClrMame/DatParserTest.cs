using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parsers.ClrMame;
using Parser = Parsers.ClrMame.Parser;

namespace CleanEmulatorFrontend.Test.ClrMame
{
    [TestClass]
    public class DatParserTest
    {
        private readonly Parser _parser = new Parser(new DatGrammar());

        [TestMethod]
        public void TestParse_clrMameHeader()
        {
            var result = _parser.Parse("ClrMame\\clrmame_header.dat");
            result.Metadata.Should().Contain("name", "Nintendo - Super Nintendo Entertainment System");
            result.Metadata.Should().Contain("description", "Nintendo - Super Nintendo Entertainment System");
            result.Metadata.Should().Contain("version", "20141025-064847");
            result.Metadata.Should().Contain("comment", "no-intro | www.no-intro.org");
        }

        [TestMethod]
        public void TestParse_verifiedGame()
        {
            var result = _parser.Parse("ClrMame\\verified_game.dat");
            result.Systems.Count.Should().Be(1);
            var snesSystem = result.Systems[0];
            snesSystem.Games.Count.Should().Be(1);
            snesSystem.Description.Should().BeNull();
            var firstGame=snesSystem.Games[0];
            firstGame.Description.Should().Be("ACME Animation Factory (USA)");
            firstGame.Roms.Count.Should().Be(1);
            firstGame.Roms[0].Name.Should().Be("ACME Animation Factory (USA).sfc");
        }

        [TestMethod]
        public void TestParse_fullDat()
        {
            var result = _parser.Parse("ClrMame\\snes.dat");
            result.Systems.Count.Should().Be(1);
            var snesSystem = result.Systems[0];
            snesSystem.Games.Count.Should().Be(3365);
        }
    }

}
