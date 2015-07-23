using System.Diagnostics;

namespace CleanEmulatorFrontend.GamesData
{
    public interface ILauncher
    {
        Process CreateGameProcess(Game game);
    }
}