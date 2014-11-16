using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using GamesData;
using GamesData.DatData;

namespace Parsers.Higan
{
    public class Library : ILibrary
    {
        private const string _extension = ".sfc";


        public void Parse(EmulatedSystem emulatedSystem)
        {
            string datPath = ConfigurationManager.AppSettings[emulatedSystem.CompatibleEmulator.RomFolderKey];

            Parse(emulatedSystem, datPath);
        }

        public void Parse(EmulatedSystem emulatedSystem, string datPath)
        {
            var directoryInfo = new DirectoryInfo(datPath);
            DirectoryInfo[] folders = directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories);


            IEnumerable<DirectoryInfo> roms = folders.Where(f => f.FullName.EndsWith(_extension));
            List<Game> games = roms.Select(rom => new Game
                                                  {
                                                      Description = (Path.GetFileNameWithoutExtension(rom.FullName)),
                                                      LaunchPath = rom.FullName,
                                                      System = emulatedSystem
                                                  }).ToList();
            emulatedSystem.Games = games;
        }
    }

    public struct HiganSystem
    {
        public const string Manufacturer = "Nintendo";
        public string LibrarySubFolder { get; set; }
        public string Description { get; set; }
        public string ShortName { get; set; }
        public string Extension { get; set; }
    }
}