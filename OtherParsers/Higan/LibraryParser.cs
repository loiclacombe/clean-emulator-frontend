using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesData.DatData;

namespace Parsers.Higan
{
    public class LibraryParser : IDatParser
    {
        public Dat Parse(string datfolder)
        {
            var directoryInfo = new DirectoryInfo(datfolder);
            var folders = directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories);

            var snesRoms=folders.Where(f => f.FullName.EndsWith(".sfc"));

            var dat = new Dat();
            var snesSystem = new EmulatedSystem()
                                     {
                                         Description = "Super Nintendo Entertainment System", Manufacturer = "Nintendo", ShortName = "SNES"
                                     };
            dat.Systems.Add(snesSystem);

            var games=snesRoms.Select(snesRom => new Game()
                                           {
                                               Description = (Path.GetFileNameWithoutExtension(snesRom.FullName)),
                                               LaunchPath = snesRom.FullName
                                           });
            snesSystem.Games = games.ToList();
            return dat;
        }
    }
}
