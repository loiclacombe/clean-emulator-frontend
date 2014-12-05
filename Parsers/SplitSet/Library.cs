using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using GamesData;

namespace Parsers.SplitSet
{
    public class Library
    {
        private const string SupportedExtension = "zip";

        public void Parse(string libraryFolderKey, EmulatedSystem emulatedSystem)
        {
            string datPath = ConfigurationManager.AppSettings[libraryFolderKey];
            if (datPath == null)
            {
                throw new InvalidDataException(string.Format("Dat library {0} is not in configuration",
                    libraryFolderKey));
            }
            var directoryInfo = new DirectoryInfo(datPath);
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