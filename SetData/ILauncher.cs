using System.Diagnostics;
using GamesData.DatData;

namespace GamesData
{
    public interface ILauncher
    {
        Process StartGame(Game game);
    }
}
