using log4net;

namespace Parsers.ClrMame
{
    public class Parser
    {
        private const string ClrMamePro = "clrmamepro";
        private const string Game = "game";
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Parser));
        private readonly DatGrammar _datGrammar;

        public Parser(DatGrammar datGrammar)
        {
            _datGrammar = datGrammar;
        }
    }
}