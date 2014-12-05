using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using GamesData;
using log4net;

namespace Launchers
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
            var emuPath = ConfigurationManager.AppSettings[_emulator.EmulatorPathKey];
            var fileInfo = new FileInfo(emuPath);
            var process = new Process
                          {
                              StartInfo =
                              {
                                  FileName = emuPath,
                                  Arguments = Arguments(game),
                                  UseShellExecute = false,
                                  RedirectStandardOutput = true,
                                  RedirectStandardError = true,
                                  WorkingDirectory = fileInfo.DirectoryName                              }
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
