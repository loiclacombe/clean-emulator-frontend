using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamesData;
using Parsers;
using Seterlund.CodeGuard;

namespace OtherParsers.SplitSet
{
    public class Library : ILibrary
    {
        private const string DefaultExtension = ".zip";


        public void Parse(GamesData.Library library, EmulatedSystem emulatedSystem)
        {
            Guard.That(library.Folder).IsNotNull();
            var directoryInfo = new DirectoryInfo(library.Folder);

            var extensions = new HashSet<string> {DefaultExtension};
            if (library.RomExtension != null)
            {
                foreach (string extension in library.RomExtension)
                {
                    extensions.Add("." + extension);
                }
            }

            IEnumerable<FileInfo> roms = directoryInfo.GetFiles(
                "*.*", SearchOption.AllDirectories)
                .Where(f => extensions.Contains(f.Extension))
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