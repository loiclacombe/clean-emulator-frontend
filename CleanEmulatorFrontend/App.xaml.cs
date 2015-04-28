using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using CleanEmulatorFrontend.Cache;
using CleanEmulatorFrontend.GamesData;
using log4net.Config;
using Launchers;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Ninject;

namespace CleanEmulatorFrontend.GUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IKernel _container;
        private MainWindow _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            XmlConfigurator.Configure();
            ConfigureContainer();
            ComposeObjects();
            Current.MainWindow.Show();
        }

        private void ConfigureContainer()
        {
            _container = new StandardKernel();
            _container.Bind<MainWindow>().ToSelf().InSingletonScope();
            _container.Bind<GenericLauncher>().ToSelf().InSingletonScope();
            _container.Bind<Directory>().ToConstant(new RAMDirectory());
            _container.Bind<Analyzer>().To<SimpleAnalyzer>().InSingletonScope();
            _container.Bind<SystemConfigRootLoader>().ToSelf().InSingletonScope();
            _container.Bind<IndexWriter.MaxFieldLength>().ToConstant(new IndexWriter.MaxFieldLength(50));
            _container.Bind<LoadedSystems>().ToSelf().InSingletonScope();
            _container.Bind<ThriftCacheManager>().ToSelf().InSingletonScope();

            _mainWindow = _container.Get<MainWindow>();
        }

        private void ComposeObjects()
        {
            Current.MainWindow = _mainWindow;
            _mainWindow.InitializeContent();
        }
    }

    public class SystemConfigRootLoader
    {
        public async Task<SystemConfigRoot> ReadEmuConfig()
        {
            SystemConfigRoot systemConfigRoot = await ReadSystemConfigRoot();
            await Task.Run(() =>
            {
                systemConfigRoot.InitializeDictionaries();
                FillEmulators(systemConfigRoot);
            });
            return systemConfigRoot;
        }

        private async Task<SystemConfigRoot> ReadSystemConfigRoot()
        {
            SystemConfigRoot systemConfigRoot;
            var assembly = Assembly.GetAssembly(typeof(SystemConfigRoot));
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
                emulator.Launcher = new GenericLauncher(emulator);
            }
        }
    }
}