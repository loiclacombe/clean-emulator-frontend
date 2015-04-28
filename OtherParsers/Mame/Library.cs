using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using CleanEmulatorFrontend.GamesData;
using ParsersBase;
using Seterlund.CodeGuard;

namespace OtherParsers.Mame
{
    public class Library : ILibrary
    {
        public EmulatedSystemSetsData Parse(CleanEmulatorFrontend.GamesData.Library library)
        {
            var emulatedSystem = new EmulatedSystemSetsData();

            using (var stream = ReadMameDat().StandardOutput)
            {
                var serializer = new XmlSerializer(typeof (MameXml));
                var mameData = serializer.Deserialize(stream) as MameXml;
                Guard.That(mameData).IsNotNull();
                emulatedSystem.Games =
                    mameData.Game.Select(g => g.AsGameDataOn()).ToList();
            }
            return emulatedSystem;
        }

        private static Process ReadMameDat()
        {
            var mamePath = ConfigurationManager.AppSettings["emulators.Mame.path"];
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
        public static CleanEmulatorFrontend.GamesData.Game AsGameDataOn(this Game xmlGame)
        {
            var game = new CleanEmulatorFrontend.GamesData.Game
            {
                Description = xmlGame.Description,
                LaunchPath = xmlGame.Name
            };
            return game;
        }
    }
}