using CleanEmulatorFrontend.GamesData;

namespace ParsersBase
{
    public interface ILibrary
    {
        EmulatedSystemSetsData Parse(Library library);
    }
}