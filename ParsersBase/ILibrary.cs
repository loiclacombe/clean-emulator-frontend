using GamesData;

namespace Parsers
{
    public interface ILibrary
    {
        void Parse(GamesData.Library library, EmulatedSystem emulatedSystem);
    }
}