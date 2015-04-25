using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesData
{
    public partial class Library
    {
        const char Separator = ';';
        public virtual string Path
        {
            get
            {
                return ConfigurationManager.AppSettings[LibraryFolderKey];
            }
        }

        public virtual string[] Paths
        {
            get
            {
                return Path.Split(Separator);
            }
        }

        public bool IsRom(FileInfo romFile)
        {
            return RomExtension.Contains(romFile.Extension);
        }
    }
}
