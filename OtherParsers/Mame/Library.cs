using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AppConfig;
using CleanEmulatorFrontend.GamesData;
using ParsersBase;
using Seterlund.CodeGuard;

namespace OtherParsers.Mame
{
    public class Library : ILibrary
    {
        private const string KeyMame = "mame";
        private readonly UserConfiguration _configuration;

        public Library(UserConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<EmulatedSystemSetsData> Parse(CleanEmulatorFrontend.GamesData.Library library)
        {
            var emulatedSystem = new EmulatedSystemSetsData();

            var mameData = await ParseMameXmlAsync(library);
            Guard.That(mameData).IsNotNull();
            emulatedSystem.Games = mameData.Machine.Select(g => g.AsGameDataOn()).ToList();
            return emulatedSystem;
        }

        private async Task<MameXml> ParseMameXmlAsync(CleanEmulatorFrontend.GamesData.Library library)
        {
            return await Task.Run(() => ParseMameXml());
        }

        private MameXml ParseMameXml()
        {
            var serializer = new XmlSerializer(typeof(MameXml));
            using (var stream = ReadMameDat().StandardOutput)
            {
                return serializer.Deserialize(stream) as MameXml;
            }
        }

        private Process ReadMameDat()
        {
            var mamePath = _configuration.Emulators.Single(e=> e.Name == KeyMame).Path;
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
        public static CleanEmulatorFrontend.GamesData.Game AsGameDataOn(this Machine xmlGame)
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