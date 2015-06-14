using System;
using System.Configuration;
using System.IO;
using System.Linq;
using AppConfig;

namespace CleanEmulatorFrontend.GamesData
{
    public partial class Library
    {
        public virtual string[] Paths { get; set; }

        public bool IsRom(FileInfo romFile)
        {
            return RomExtension.Contains(romFile.Extension);
        }

        public bool IsValid()
        {
            return Paths != null && Paths.Count()!=0 ;
        }
    }
}