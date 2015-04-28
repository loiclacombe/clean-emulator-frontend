using System.Diagnostics;

namespace CleanEmulatorFrontend.GamesData
{
    public interface ILauncher
    {
        Process StartGame(Game game);
    }
}