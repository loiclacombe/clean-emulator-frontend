using Irony.Parsing;

namespace Parsers.ClrMame
{
    [Language("ClrMame", "1.0", "ClrMame data format")]
    public class DatGrammar : Grammar
    {
        public const string Name = "Name";
        public const string Text = "Text";
        public const string Number = "Number";
        public const string Hash = "Hash";
        public const string Date = "Date";
        public const string Constant = "Constant";
        private const string List = "List";
        private const string Property = "Property";
        public static readonly string[] ValueTypes = {Text, Number, Hash, Date, Constant};

        public DatGrammar()
        {
            var name = new IdentifierTerminal(Name);
            var text = new StringLiteral(Text, "\"");
            var number = new NumberLiteral(Number);
            var hash = new RegexBasedTerminal(Hash, "([0-9A-Fa-f]+)");
            var date = new RegexBasedTerminal(Date, "(([0-9]+-?)+)");
            var constant = new RegexBasedTerminal(Constant, "(verified|baddump)");

            var value = new NonTerminal("Value");

            var prop = new NonTerminal(Property);
            var list = new NonTerminal(List);
            var listBr = new NonTerminal("ListBr");

            value.Rule = text | number | date | hash | constant | listBr;

            prop.Rule = name + value;
            list.Rule = MakePlusRule(list, prop);
            listBr.Rule = "(" + list + ")";


            MarkPunctuation("(", ")", "\"");
            MarkTransient(listBr, value);
            Root = list;
        }
    }
}