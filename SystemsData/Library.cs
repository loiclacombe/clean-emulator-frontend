using System.Configuration;
using System.IO;
using System.Linq;

namespace CleanEmulatorFrontend.GamesData
{
    public partial class Library
    {
        private const char Separator = ';';

        public virtual string Path
        {
            get { return ConfigurationManager.AppSettings[LibraryFolderKey]; }
        }

        public virtual string[] Paths
        {
            get { return Path.Split(Separator); }
        }

        public bool IsRom(FileInfo romFile)
        {
            return RomExtension.Contains(romFile.Extension);
        }
    }
}