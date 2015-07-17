using System.Collections.Generic;
using System.Threading.Tasks;
using CleanEmulatorFrontend.GamesData;
using log4net;
using ParsersBase;
using Seterlund.CodeGuard;
using Library = OtherParsers.SplitSet.Library;

namespace CleanEmulatorFrontend.GUI
{
    public class FromDatsGamesDataLoader
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FromDatsGamesDataLoader));
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
                var emulatedSystemSetsData = await library.Parse(libraryData);
                Logger.DebugFormat("Found {0} games in {1}", emulatedSystemSetsData.Games.Count, compatibleEmulator.LibraryName);
                return emulatedSystemSetsData;
            }
            return new EmulatedSystemSetsData();
        }
    }
}