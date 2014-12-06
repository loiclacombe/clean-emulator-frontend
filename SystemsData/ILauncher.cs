using System.Diagnostics;

namespace GamesData
{
    public interface ILauncher
    {
        Process StartGame(Game game);
    }
}
