using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AppConfig;
using Castle.Core.Internal;
using CleanEmulatorFrontend.GamesData;
using Launchers;
using Library = AppConfig.Library;

namespace CleanEmulatorFrontend.GUI
{
    public class SystemConfigRootLoader
    {
        private UserConfiguration _userConfiguration;

        public SystemConfigRootLoader(UserConfiguration userConfiguration)
        {
            _userConfiguration = userConfiguration;
        }

        public async Task<SystemConfigRoot> 
            ReadEmuConfig()
        {
            var systemConfigRoot = await ReadSystemConfigRoot();
            systemConfigRoot.Library.ForEach(
                l =>
                {
                    var foundLibrary = _userConfiguration.FindLibraryByKey(l.LibraryFolderKey);
                    if (foundLibrary != null)
                    {
                        l.Paths = foundLibrary.Path;
                    }
                });
            await Task.Run(() =>
            {
                systemConfigRoot.InitializeDictionaries();
                FillEmulators(systemConfigRoot);
                FillEmulatorsInSystems(systemConfigRoot);
                FillLibraries(systemConfigRoot);
            });
            return systemConfigRoot;
        }

        private void FillLibraries(SystemConfigRoot systemConfigRoot)
        {
            
        }

        private void FillEmulatorsInSystems(SystemConfigRoot systemConfigRoot)
        {
            foreach (var emulatedSystem in systemConfigRoot.AllSystems)
            {
                emulatedSystem.Emulator = systemConfigRoot.EmulatorsDict[emulatedSystem.CompatibleEmulator.Name];
            }
        }

        private async Task<SystemConfigRoot> ReadSystemConfigRoot()
        {
            SystemConfigRoot systemConfigRoot;
            var assembly = Assembly.GetAssembly(typeof (SystemConfigRoot));
            using (var stream = assembly.GetManifestResourceStream("CleanEmulatorFrontend.GamesData.emuconfig.xml"))
            {
                var serializer = new XmlSerializer(typeof (SystemConfigRoot));
                systemConfigRoot = serializer.Deserialize(stream) as SystemConfigRoot;
            }

            return systemConfigRoot;
        }

        private void FillEmulators(SystemConfigRoot systemConfigRoot)
        {
            foreach (var emulator in systemConfigRoot.Emulator)
            {
                emulator.Launcher = new GenericLauncher(emulator, _userConfiguration);
            }
        }
    }
}