using GamesData.DatData;

namespace Parsers
{
    public interface IDatParser
    {
        LibraryData Parse(string datPath);
    }
}