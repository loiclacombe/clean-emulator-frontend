using GamesData;

namespace Parsers
{
    public interface ILibrary
    {
        void Parse(string libraryFolderKey,
            EmulatedSystem emulatedSystem);
    }
}