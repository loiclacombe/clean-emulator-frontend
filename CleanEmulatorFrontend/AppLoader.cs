using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using CleanEmulatorFrontend.Engine.Data;
using CleanEmulatorFrontend.Engine.Launchers;
using GamesData.DatData;
using ClrMame = Parsers.ClrMame;
using Higan = Parsers.Higan;

namespace CleanEmulatorFrontend
{
    public class AppLoader
    {
        private ClrMame.Parser _clrMameParser;
        private Higan.Library _higanLibrary;

        public AppLoader(ClrMame.Parser clrMameParser, Higan.Library higanLibrary)
        {
            _higanLibrary = higanLibrary;
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
           emulatedSystems.AddRange(_higanLibrary.Parse().Systems);
        }
    }
}