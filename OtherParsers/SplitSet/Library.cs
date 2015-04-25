using System.IO;
using System.Linq;
using GamesData;
using Parsers;
using Seterlund.CodeGuard;

namespace OtherParsers.SplitSet
{
    public class Library : ILibrary
    {

        public void Parse(GamesData.Library library, EmulatedSystem emulatedSystem)
        {
            Guard.That(library.Path).IsNotNull();

            library.Paths.Select(p => new DirectoryInfo(p))
                .Select(di => new
                {
                    LibraryPath = di.FullName,
                    files = di.GetFiles("*.*", SearchOption.AllDirectories)
                    .Where(f=> library.RomExtension.Contains(f.Extension) && IsNotBios(f))
                })
                .SelectMany(topDir => topDir.files.Select(file => new { TopDir = topDir.LibraryPath, file }))
                .AsParallel()
                .ForAll(rom => emulatedSystem.Games
                        .Add(new Game
                        {
                            Description =
                                   (Path.GetFileNameWithoutExtension(rom.file.FullName)),
                            BasePath = rom.TopDir,
                            LaunchPath = RemoveLibraryPathFrom(rom.file, rom.TopDir),
                            System = emulatedSystem
                        }));
        }

        private static string RemoveLibraryPathFrom(FileInfo rom, string libDir)
        {
            return rom.FullName.Replace((libDir + Path.PathSeparator+Path.PathSeparator), "");
        }

        private static bool IsNotBios(FileInfo r)
        {
            return !r.FullName.Contains("[BIOS]");
        }
    }
}