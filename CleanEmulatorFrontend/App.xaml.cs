using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;
using GamesData;
using log4net.Config;
using Launchers;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Ninject;
using Ninject.Activation;
using Directory = Lucene.Net.Store.Directory;

namespace CleanEmulatorFrontend
{
    /// <summary>
    /// Interaction logic for App.xaml
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
            _container.Bind<SystemConfigRoot>().ToProvider(new SystemConfigRootProvider());
            _container.Bind<IndexWriter.MaxFieldLength>().ToConstant(new IndexWriter.MaxFieldLength(50));
            _container.Bind<LoadedSystems>().ToSelf().InSingletonScope();
            _mainWindow = _container.Get<MainWindow>();
        }

        private void ComposeObjects()
        {
            Current.MainWindow = _mainWindow;
            _mainWindow.InitializeContent();
        }
    }

    public class SystemConfigRootProvider : IProvider<SystemConfigRoot>
    {

        private SystemConfigRoot ReadEmuConfig()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(SystemConfigRoot));
            using (Stream stream = assembly.GetManifestResourceStream("GamesData.emuconfig.xml"))
            {
                var serializer = new XmlSerializer(typeof(SystemConfigRoot));
                var systemConfigRoot = serializer.Deserialize(stream) as SystemConfigRoot;
                return systemConfigRoot;
            }
        }

        public object Create(IContext context)
        {
            return ReadEmuConfig();
        }

        public Type Type { get; private set; }
    }



}
