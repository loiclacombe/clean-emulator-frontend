using System.Collections.Generic;
using System.Threading.Tasks;
using CleanEmulatorFrontend.GamesData;
using ParsersBase;
using Seterlund.CodeGuard;
using Library = OtherParsers.SplitSet.Library;

namespace CleanEmulatorFrontend.GUI
{
    public class FromDatsGamesDataLoader
    {
        private readonly Dictionary<string, ILibrary> _librariesParser;

        public FromDatsGamesDataLoader(Library splitSetLibrary,OtherParsers.Mame.Library mameLibrary)
        {
            _librariesParser = new Dictionary<string, ILibrary>
            {
                {typeof (Library).FullName, splitSetLibrary},
                {typeof (OtherParsers.Mame.Library).FullName, mameLibrary}
            };
        }

        public async Task<EmulatedSystemSetsData> LoadGamesData(SystemConfigRoot systemConfigRoot, CompatibleEmulator compatibleEmulator)
        {
            var libraryData = systemConfigRoot.LibrariesDict[compatibleEmulator.LibraryName];
            Guard.That(libraryData).IsNotNull();

            var library = _librariesParser[libraryData.LibraryClass];

            if (library != null)
            {
                return await library.Parse(libraryData);
            }
            return new EmulatedSystemSetsData();
        }
    }
}