using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using GamesData;
using Seterlund.CodeGuard;

namespace Parsers.Higan
{
    public class Library
    {
        private const string Extension = ".sfc";

        public void Parse(string libraryFolderKey,
            EmulatedSystem emulatedSystem)
        {
            string datPath = ConfigurationManager.AppSettings[libraryFolderKey];
            Guard.That(datPath).IsNotNull();

            Parse(emulatedSystem, datPath);
        }

        public void Parse(EmulatedSystem emulatedSystem, string datPath)
        {
            var directoryInfo = new DirectoryInfo(datPath);
            DirectoryInfo[] folders = directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories);


            IEnumerable<DirectoryInfo> roms = folders.Where(IsSfcRom());
            var games = roms.Select(rom => new Game
                                                  {
                                                      Description = (Path.GetFileNameWithoutExtension(rom.FullName)),
                                                      LaunchPath = rom.FullName,
                                                      System = emulatedSystem
                                                  }).ToList();
            games.ForEach(emulatedSystem.Games.Add);
        }

        private static Func<DirectoryInfo, bool> IsSfcRom()
        {
            return f => f.FullName.EndsWith(Extension);
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