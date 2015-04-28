using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanEmulatorFrontend.GamesData;
using Irony.Parsing;
using log4net;
using Parsers.ClrMame;
using ParsersBase;
using Parser = Irony.Parsing.Parser;

namespace ClrMameParser
{
    public class Library : ILibrary
    {
        private const string ClrMamePro = "clrmamepro";
        private const string Game = "game";
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Library));
        private readonly DatGrammar _datGrammar;

        public Library(DatGrammar datGrammar)
        {
            _datGrammar = datGrammar;
        }

        public EmulatedSystemSetsData Parse(CleanEmulatorFrontend.GamesData.Library library)
        {
            var emulatedSystem = new EmulatedSystemSetsData
            {
                Games = new List<Game>()
            };
            Logger.DebugFormat("Parsing {0}", library.Path);
            var parser = new Parser(_datGrammar);
            using (var reader = new StreamReader(new FileStream(library.Path, FileMode.Open)))
            {
                var parseTree = parser.Parse(reader.ReadToEnd());
                parseTree.ParserMessages.ForEach(pm => { Console.WriteLine("{0} {1}", pm.Message, pm.Location); });
                var parseTreeNode = parseTree.Root;

                var rootList = parseTreeNode.ChildNodes;


                foreach (var node in rootList)
                {
                    TryParseClrMameBlock(node, emulatedSystem);
                    TryParseGameBlock(node, emulatedSystem);
                }
            }
            return emulatedSystem;
        }

        private void TryParseClrMameBlock(ParseTreeNode node, EmulatedSystemSetsData emulatedSystem)
        {
            if (PropertyName(node) != ClrMamePro)
            {
                return;
            }

            emulatedSystem.LibraryMetadata = ExtractValueProperties(node);
        }

        private void TryParseGameBlock(ParseTreeNode gameNode, EmulatedSystemSetsData emulatedSystem)
        {
            if (PropertyName(gameNode) != Game)
            {
                return;
            }

            var properties = ExtractValueProperties(gameNode);
            var roms = new List<Rom>();
            var game = new Game
            {
                Description = properties["description"],
                LaunchPath = properties["name"]
            };

            roms.AddRange(ValueListNodes(gameNode).Where(IsRom).Select(ParseRom));
            emulatedSystem.Games.Add(game);
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
            foreach (var metadata in ValueListNodes(propertyNode).Where(IsValueType))
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