using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using GamesData;
using Seterlund.CodeGuard;

namespace Parsers.Mame
{
    public class Library: ILibrary
    {
        public void Parse(string libraryFolder, EmulatedSystem emulatedSystem)
        {
            using (StreamReader stream = ReadMameDat().StandardOutput)
            {
                var serializer = new XmlSerializer(typeof (Mame));
                var mameData = serializer.Deserialize(stream) as Mame;
                Guard.That(mameData).IsNotNull();
                mameData.Game
                    .Select(g => g.AsGameDataOn(emulatedSystem))
                    .AsParallel()
                    .ForAll(
                        g => emulatedSystem.Games.Add(g)
                    );

            }
        }

        private static Process ReadMameDat()
        {
            string mamePath = ConfigurationManager.AppSettings["emulators.Mame.path"];
            var mameInfo = new FileInfo(mamePath);
            var process = new Process
                          {
                              StartInfo =
                              {
                                  FileName = mamePath,
                                  Arguments = "-listxml",
                                  UseShellExecute = false,
                                  CreateNoWindow = true,
                                  RedirectStandardOutput = true,
                                  WorkingDirectory = mameInfo.DirectoryName
                              }
                          };
            process.Start();
            return process;
        }
    }

    public static class Utils
    {
        public static GamesData.Game AsGameDataOn(this Game xmlGame, EmulatedSystem emulatedSystem)
        {
            var game = new GamesData.Game
                       {
                           Description = xmlGame.Description,
                           LaunchPath = xmlGame.Name,
                           System = emulatedSystem
                       };
            return game;
        }
    }
}