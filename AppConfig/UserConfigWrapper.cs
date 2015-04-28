using System;
using System.Configuration;
using System.IO;

namespace AppConfig
{
    public class UserConfigWrapper
    {
        public string ClrMameDatFolder
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "CleanEmulatorFrontend",
                    "Dats");
            }
        }

        public string HiganLibrary
        {
            get { return ConfigurationManager.AppSettings["emulators.higan.library"]; }
        }
    }
}