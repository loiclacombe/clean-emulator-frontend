using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using CleanEmulatorFrontend.Engine.Data;
using GamesData.DatData;
using Parsers.Higan;
using ClrMame = Parsers.ClrMame;

namespace CleanEmulatorFrontend
{
    public class AppLoader
    {
        private readonly string _datFolder;
        private DirectoryInfo[] directories;
        private ClrMame.DatParser _datParser;

        public AppLoader(ClrMame.DatParser datParser )
        {
            _datFolder = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "CleanEmulatorFrontend",
                                      "Dats");
            _datParser = datParser;
        }

        public IEnumerable<SystemGroup> LoadDats()
        {


            var emulatedSystems = new List<EmulatedSystem>();
            var consoles = new SystemGroup
            {
                Name = "Consoles",
                Systems=emulatedSystems
            };

            //LoadClrMameDats(emulatedSystems);
            LoadHiganFolders(emulatedSystems);

            IEnumerable<SystemGroup> groups = new SortedSet<SystemGroup>
                                                  {
                                                      consoles
                                                  };

            return groups;
        }

        private void LoadHiganFolders(List<EmulatedSystem> emulatedSystems)
        {
           var libraryParser=new LibraryParser();
           emulatedSystems.AddRange(libraryParser.Parse(ConfigurationManager.AppSettings["emulators.higan.library"]).Systems);

        }

        private void LoadClrMameDats(List<EmulatedSystem> emulatedSystems)
        {
            var directoryInfo = new DirectoryInfo(_datFolder);
            var files = directoryInfo.GetFiles("*.dat", SearchOption.AllDirectories);
            foreach (var fileInfo in files)
            {
                var dat = _datParser.Parse(fileInfo.FullName);
                emulatedSystems.AddRange(dat.Systems);
            }
        }
    }
}