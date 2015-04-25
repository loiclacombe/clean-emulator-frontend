using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                                  Arguments = FormatArguments(game),
                                  UseShellExecute = false,
                                  RedirectStandardOutput = true,
                                  RedirectStandardError = true,
                                  WorkingDirectory = fileInfo.DirectoryName                              }
                          };
            process.Start();

            return process;
        }

        private string FormatArguments(Game game)
        {
            List<string> parameters = new List<string> {game.AbsoluteLaunchPath};
            if (_emulator.ParametersFromKeys != null)
            {
                parameters.AddRange(_emulator.ParametersFromKeys.Select(p => ConfigurationManager.AppSettings[p]));
            }

            return string.Format(_emulator.CliParameters, parameters.ToArray());
        }
    }

}
