using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AppConfig;
using CleanEmulatorFrontend.GamesData;
using log4net;
using Emulator = CleanEmulatorFrontend.GamesData.Emulator;

namespace Launchers
{
    public class GenericLauncher : ILauncher
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (GenericLauncher));
        private readonly Emulator _emulator;
        private readonly UserConfiguration _userConfiguration;

        public GenericLauncher(Emulator emulator, UserConfiguration userConfiguration)
        {
            _emulator = emulator;
            _userConfiguration = userConfiguration;
        }

        public Process StartGame(Game game)
        {
            Logger.InfoFormat("Starting {0}", game);

            var emuPath = _userConfiguration.Emulators.Single(e => e.Name == _emulator.EmulatorPathKey).Path;
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
                    WorkingDirectory = fileInfo.DirectoryName
                }
            };
            process.Start();

            return process;
        }

        private string FormatArguments(Game game)
        {
            var parameters = new List<string> {game.AbsoluteLaunchPath};
            if (_emulator.ParametersFromKeys != null)
            {

                var confValues=(from ck in _userConfiguration.ConfigurationKeys
                join pk in _emulator.ParametersFromKeys on ck.Name equals pk
                select ck.Value);
                parameters.AddRange(confValues);
            }

            return string.Format(_emulator.CliParameters, parameters.ToArray());
        }
    }
}