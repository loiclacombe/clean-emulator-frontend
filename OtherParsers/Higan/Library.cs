using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamesData;
using Parsers;
using Seterlund.CodeGuard;

namespace OtherParsers.Higan
{
    public class Library : ILibrary
    {
        private const string Extension = ".sfc";

        public void Parse(GamesData.Library library, EmulatedSystem emulatedSystem)
        {
            Guard.That(library.Path).IsNotNull();
            var directoryInfo = new DirectoryInfo(library.Path);
            DirectoryInfo[] folders = directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories);


            IEnumerable<DirectoryInfo> roms = folders.Where(IsSfcRom());
            var games = roms.Select(rom => new Game
            {
                Description = (Path.GetFileNameWithoutExtension(rom.FullName)),
                LaunchPath = rom.FullName,
                BasePath = library.Path,
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