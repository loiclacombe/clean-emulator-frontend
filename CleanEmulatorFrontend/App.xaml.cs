using System;
using System.Configuration;
using System.IO;
using System.Windows;
using AppConfig;
using CleanEmulatorFrontend.Cache;
using CleanEmulatorFrontend.Cache.Thrift;
using CleanEmulatorFrontend.SqLiteCache;
using log4net;
using log4net.Config;
using Launchers;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Ninject;
using Ninject.Activation;
using Directory = Lucene.Net.Store.Directory;

namespace CleanEmulatorFrontend.GUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string KeyGamesCacheEntities = "GamesCacheEntities";
        private static readonly ILog Logger = LogManager.GetLogger(typeof (App));
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
            try
            {
                _container = new StandardKernel();
                _container.Bind<MainWindow>().ToSelf().InSingletonScope();
                _container.Bind<GenericLauncher>().ToSelf().InSingletonScope();
                _container.Bind<Directory>().ToConstant(new RAMDirectory());
                _container.Bind<Analyzer>().To<SimpleAnalyzer>().InSingletonScope();
                _container.Bind<SystemConfigRootLoader>().ToSelf().InSingletonScope();
                _container.Bind<IndexWriter.MaxFieldLength>().ToConstant(new IndexWriter.MaxFieldLength(50));
                _container.Bind<LoadedSystems>().ToSelf().InSingletonScope();
                _container.Bind<ICacheManager>().To<ThriftCacheManager>().InSingletonScope();
                _container.Bind<GamesCacheEntities>().ToMethod(BuildGamesCacheEntities);
                _container.Bind<UserConfiguration>().ToMethod(BuildUserConfiguration).InSingletonScope();

                _mainWindow = _container.Get<MainWindow>();
            }
            catch (Exception e)
            {
                Logger.Error("Injection initialization failed", e);
                throw;
            }
        }

        private UserConfiguration BuildUserConfiguration(IContext arg)
        {
            var configuration = new UserConfiguration();
            configuration.Load();
            return configuration;
        }

        private GamesCacheEntities BuildGamesCacheEntities(IContext context)
        {
            var connectionString = Environment.ExpandEnvironmentVariables(
                ConfigurationManager.ConnectionStrings[KeyGamesCacheEntities].ConnectionString);
            return new GamesCacheEntities(connectionString);
        }

        private void ComposeObjects()
        {
            Current.MainWindow = _mainWindow;
            _mainWindow.InitializeContent();
        }
    }
}