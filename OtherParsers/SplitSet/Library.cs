using System.IO;
using System.Linq;
using CleanEmulatorFrontend.GamesData;
using ParsersBase;
using Seterlund.CodeGuard;

namespace OtherParsers.SplitSet
{
    public class Library : ILibrary
    {
        public EmulatedSystemSetsData Parse(CleanEmulatorFrontend.GamesData.Library library)
        {
            Guard.That(library.Paths).IsNotNull();
            var emulatedSystem = new EmulatedSystemSetsData();
            var librariesDirs = library.Paths.Select(p => new DirectoryInfo(p))
                .Select(di => new
                {
                    LibraryPath = di.FullName,
                    files = di.GetFiles("*.*", SearchOption.AllDirectories)
                        .Where(f => library.IsRom(f) && IsNotBios(f))
                });
            var romData = librariesDirs
                .SelectMany(topDir => topDir.files.Select(file => new {TopDir = topDir.LibraryPath, file}));
            emulatedSystem.Games = romData.Select(rom => new Game
            {
                Description =
                    (Path.GetFileNameWithoutExtension(rom.file.FullName)),
                BasePath = rom.TopDir,
                LaunchPath = RemoveLibraryPathFrom(rom.file, rom.TopDir)
            }).ToList();
            return emulatedSystem;
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
}