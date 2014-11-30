﻿using System.Configuration;
using System.IO;
using System.Linq;
using GamesData;
using GamesData.DatData;

namespace Parsers.SplitSet
{
    public class Library : ILibrary
    {
        private const string SupportedExtension = "zip";
        private const string IgnoreRegex = "zip";

        public void Parse(EmulatedSystem emulatedSystem)
        {
            var datPath = ConfigurationManager.AppSettings[emulatedSystem.CompatibleEmulator.RomFolderKey];
            if (datPath == null)
            {
                throw new InvalidDataException(string.Format("Dat library {0} is not in configuration", emulatedSystem.CompatibleEmulator.RomFolderKey));
            }
            Parse(emulatedSystem, datPath);
        }

        public void Parse(EmulatedSystem emulatedSystem, string datPath)
        {
            var directoryInfo = new DirectoryInfo(datPath);
            var roms = directoryInfo.GetFiles("*." + SupportedExtension, SearchOption.AllDirectories)
                .Where(r => !IsBios(r));
            var games = roms.Select(rom => new Game()
                                           {
                                               Description = (Path.GetFileNameWithoutExtension(rom.FullName)),
                                               LaunchPath = rom.FullName,
                                               System = emulatedSystem
                                           });

            emulatedSystem.Games = games.ToList();
        }

        private static bool IsBios(FileInfo r)
        {
            return r.FullName.Contains("[BIOS]");
        }
    }
}
