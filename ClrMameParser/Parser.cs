using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamesData;
using GamesData.DatData;
using Irony.Parsing;
using log4net;

namespace Parsers.ClrMame
{
    public class Parser
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Parser));
        private readonly DatGrammar _datGrammar;
        private const string ClrMamePro = "clrmamepro";
        private const string Game = "game";

        public Parser(DatGrammar datGrammar)
        {
            _datGrammar = datGrammar;
        }

    
    }
}
