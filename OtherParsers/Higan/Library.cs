using System;
using System.IO;
using System.Linq;
using CleanEmulatorFrontend.GamesData;
using ParsersBase;
using Seterlund.CodeGuard;

namespace OtherParsers.Higan
{
    public class Library : ILibrary
    {
        private const string Extension = ".sfc";

        public EmulatedSystemSetsData Parse(CleanEmulatorFrontend.GamesData.Library library)
        {
            Guard.That(library.Path).IsNotNull();
            var emulatedSystem = new EmulatedSystemSetsData();
            var directoryInfo = new DirectoryInfo(library.Path);
            var folders = directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories);


            var roms = folders.Where(IsSfcRom());
            emulatedSystem.Games = roms.Select(rom => new Game
            {
                Description = (Path.GetFileNameWithoutExtension(rom.FullName)),
                LaunchPath = rom.FullName,
                BasePath = library.Path
            }).ToList();
            return emulatedSystem;
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