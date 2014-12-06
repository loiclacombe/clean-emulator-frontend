using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using GamesData;
using Seterlund.CodeGuard;

namespace Parsers.SplitSet
{
    public class Library : ILibrary
    {
        private const string SupportedExtension = "zip";

        public void Parse(string libraryFolder, EmulatedSystem emulatedSystem)
        {
            Guard.That(libraryFolder).IsNotNull();
            var directoryInfo = new DirectoryInfo(libraryFolder);
            IEnumerable<FileInfo> roms = directoryInfo.GetFiles(
                "*." + SupportedExtension, SearchOption.AllDirectories)
                .Where(r => !IsBios(r));
            roms.AsParallel().ForAll(rom => emulatedSystem.Games
                .Add(new Game
                {
                    Description =
                        (Path.GetFileNameWithoutExtension(rom.FullName)),
                    LaunchPath = rom.FullName,
                    System = emulatedSystem
                }));
        }

        private static bool IsBios(FileInfo r)
        {
            return r.FullName.Contains("[BIOS]");
        }
    }
}