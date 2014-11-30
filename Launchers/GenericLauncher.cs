using System.Configuration;
using System.Diagnostics;
using GamesData;
using GamesData.DatData;
using log4net;

namespace EmulatorModules
{
    public class GenericLauncher : ILauncher
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GenericLauncher));
        private readonly Emulator _emulator;

        public GenericLauncher(Emulator emulator)
        {
             _emulator = emulator;
        }

        public Process StartGame(Game game)
        {
            Logger.InfoFormat("Starting {0}", game);
            var process = new Process
                          {
                              StartInfo =
                              {
                                  UseShellExecute = true,
                                  FileName = ConfigurationManager.AppSettings[_emulator.EmulatorPathKey],
                                  Arguments = Arguments(game)
                              }
                          };
            process.Start();
            return process;
        }

        private string Arguments(Game game)
        {
            return string.Format(_emulator.CliParameters, game.LaunchPath);
        }
    }

}
