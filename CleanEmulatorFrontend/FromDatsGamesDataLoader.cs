using System.Collections.Generic;
using CleanEmulatorFrontend.GamesData;
using ParsersBase;
using Seterlund.CodeGuard;
using Library = OtherParsers.SplitSet.Library;

namespace CleanEmulatorFrontend.GUI
{
    public class FromDatsGamesDataLoader
    {
        private readonly Dictionary<string, ILibrary> _librariesParser;

        public FromDatsGamesDataLoader(Library splitSetLibrary,
            OtherParsers.Higan.Library higanLibrary, OtherParsers.Mame.Library mameLibrary)
        {
            _librariesParser = new Dictionary<string, ILibrary>
            {
                {typeof (Library).FullName, splitSetLibrary},
                {typeof (OtherParsers.Higan.Library).FullName, higanLibrary},
                {typeof (OtherParsers.Mame.Library).FullName, mameLibrary}
            };
        }

        public EmulatedSystemSetsData LoadGamesData(SystemConfigRoot systemConfigRoot, CompatibleEmulator compatibleEmulator)
        {
            var libraryData = systemConfigRoot.LibrariesDict[compatibleEmulator.LibraryName];
            Guard.That(libraryData).IsNotNull();

            var library = _librariesParser[libraryData.LibraryClass];

            if (library != null)
            {
                return library.Parse(libraryData);
            }
            return new EmulatedSystemSetsData();
        }
    }
}