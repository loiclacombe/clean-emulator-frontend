using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Launchers;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Factory;

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
            _container.Bind<IndexWriter.MaxFieldLength>().ToConstant(new IndexWriter.MaxFieldLength(50));

            _mainWindow = _container.Get<MainWindow>();
        }

        private void ComposeObjects()
        {
            Current.MainWindow = _mainWindow;
            _mainWindow.InitializeContent();
        }
    }



}
