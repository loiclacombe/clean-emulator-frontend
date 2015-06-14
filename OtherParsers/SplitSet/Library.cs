using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppConfig;
using CleanEmulatorFrontend.GamesData;
using ParsersBase;
using Seterlund.CodeGuard;

namespace OtherParsers.SplitSet
{
    public class Library : ILibrary
    {
        private readonly UserConfiguration _configuration;

        public Library(UserConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<EmulatedSystemSetsData> Parse(CleanEmulatorFrontend.GamesData.Library library)
        {
            Guard.That(library.Paths).IsNotNull();
            var emulatedSystem = new EmulatedSystemSetsData();
            var romData = await ExtractRomData(library);
            emulatedSystem.Games = romData.Select(rom => new Game
            {
                Description = Path.GetFileNameWithoutExtension(rom.File.FullName),
                BasePath = rom.TopDir,
                LaunchPath = RemoveLibraryPathFrom(rom.File, rom.TopDir)
            }).ToList();
            return emulatedSystem;
        }

        private async Task<IEnumerable<RomData>> ExtractRomData(CleanEmulatorFrontend.GamesData.Library library)
        {
            return await Task.Run(() =>
            {
                var librariesDirs = library.Paths.Select(p => new DirectoryInfo(p))
                        .Select(di => new
                        {
                            LibraryPath = di.FullName,
                            Files = di.GetFiles("*.*", SearchOption.AllDirectories)
                                .Where(f => library.IsRom(f) && IsNotBios(f))
                        });
                var romData = librariesDirs.SelectMany(
                        topDir => topDir.Files.Select(file => new RomData {TopDir = topDir.LibraryPath, File = file})
                        );
                return romData;
            });
        }

        private static string RemoveLibraryPathFrom(FileInfo rom, string libDir)
        {
            return rom.FullName.Replace((libDir + Path.DirectorySeparatorChar), "");
        }

        private static bool IsNotBios(FileInfo r)
        {
            return !r.FullName.Contains("[BIOS]");
        }
    }

    struct RomData
    {
        public string TopDir { get; set; }
        public FileInfo File { get; set; }
    }
}