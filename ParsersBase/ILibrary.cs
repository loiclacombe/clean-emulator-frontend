using System.Threading.Tasks;
using AppConfig;
using CleanEmulatorFrontend.GamesData;
using Library = CleanEmulatorFrontend.GamesData.Library;

namespace ParsersBase
{
    public interface ILibrary
    {
        Task<EmulatedSystemSetsData> Parse(Library library);
    }
}