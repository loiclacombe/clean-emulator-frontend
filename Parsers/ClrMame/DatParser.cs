using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamesData.DatData;
using Irony.Parsing;
using log4net;

namespace Parsers.ClrMame
{
    public class DatParser : IDatParser
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DatParser));
        private DatGrammar _datGrammar;
        private const string ClrMamePro = "clrmamepro";
        private const string Game = "game";

        public DatParser(DatGrammar datGrammar)
        {
            _datGrammar = datGrammar;
        }

        public Dat Parse(string datfolder)
        {
            Logger.DebugFormat("Parsing {0}", datfolder);
            var parser = new Parser(_datGrammar);
            var dat = new Dat();
            using (var reader = new StreamReader( new FileStream(datfolder, FileMode.Open)))
            {
                ParseTree parseTree = parser.Parse(reader.ReadToEnd());
                parseTree.ParserMessages.ForEach(pm => { Console.WriteLine(string.Format("{0} {1}", pm.Message, pm.Location)); });
                ParseTreeNode parseTreeNode = parseTree.Root;

                ParseTreeNodeList rootList = parseTreeNode.ChildNodes;

                dat.Systems=new List<EmulatedSystem>()
                                {
                                    new EmulatedSystem()
                                        {
                                            ShortName = Path.GetFileNameWithoutExtension(datfolder),
                                        }
                                };

                foreach (ParseTreeNode node in rootList)
                {
                    TryParseClrMameBlock(node, dat);
                    TryParseGameBlock(node, dat);
                }
                return dat;
            }
        }

        private void TryParseClrMameBlock(ParseTreeNode node, Dat dat)
        {
            if (PropertyName(node) != ClrMamePro)
            {
                return;
            }

            dat.Metadata = ExtractValueProperties(node);
            dat.Systems[0].Description = dat.Metadata["description"];
        }

        private void TryParseGameBlock(ParseTreeNode gameNode, Dat dat)
        {
            if (PropertyName(gameNode) != Game)
            {
                return;
            }

            IDictionary<string, string> properties = ExtractValueProperties(gameNode);
            var roms = new List<Rom>();
            var game = new Game
                           {
                               Description = properties["description"],
                               LaunchPath = properties["name"],
                               Roms = roms
                           };

            roms.AddRange(ValueListNodes(gameNode).Where(IsRom).Select(ParseRom));
            dat.Systems[0].Games.Add(game);
        }

        private static Rom ParseRom(ParseTreeNode romNode)
        {
            IDictionary<string, string> properties;
            properties = ExtractValueProperties(romNode);
            var rom = new Rom
                          {
                              Name = properties["name"]
                          };
            return rom;
        }

        private static IDictionary<string, string> ExtractValueProperties(ParseTreeNode propertyNode)
        {
            IDictionary<string, string> valueProperties = new Dictionary<string, string>();
            foreach (ParseTreeNode metadata in ValueListNodes(propertyNode).Where(IsValueType))
            {
                valueProperties[PropertyName(metadata)] = PropertyText(metadata);
            }
            return valueProperties;
        }

        private static ParseTreeNodeList ValueListNodes(ParseTreeNode propertyNode)
        {
            return propertyNode.ChildNodes[1].ChildNodes;
        }

        private static bool IsValueType(ParseTreeNode m)
        {
            return DatGrammar.ValueTypes.Contains(PropertyValueType(m));
        }

        private static bool IsRom(ParseTreeNode m)
        {
            return PropertyName(m) == "rom";
        }

        private static string PropertyText(ParseTreeNode metadata)
        {
            return PropertyRawValue(metadata).Token.ValueString;
        }

        private static string PropertyValueType(ParseTreeNode metadata)
        {
            return PropertyRawValue(metadata).Term.Name;
        }

        private static string PropertyName(ParseTreeNode node)
        {
            return node.ChildNodes[0].FindTokenAndGetText();
        }

        private static ParseTreeNode PropertyRawValue(ParseTreeNode node)
        {
            return node.ChildNodes[1];
        }
    }
}