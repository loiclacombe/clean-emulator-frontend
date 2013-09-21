
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CleanEmulatorFrontend.Engine.Data;
using CleanEmulatorFrontend.Engine.Listers;
using Ninject;
using Ninject.Modules;


namespace CleanEmulatorFrontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SystemLister _systemLister;
        private List<Engine.Data.Game> _games=new List<Game>();

        public MainWindow()
        {
            IKernel kernel = new StandardKernel(new INinjectModule[]
                                                    {
                                                        new CEFModule()
                                                    });
            _systemLister = kernel.Get<SystemLister>();
            InitializeComponent();
            FillTreeView();
        }

        private void ClickSystem(Object o, MouseButtonEventArgs eventArgs)
        {
            var system = (Engine.Data.System) o;
            _games.Clear();
            _games.AddRange(system.);
        }

        private void FillTreeView()
        {
            TreeViewItem root = (TreeViewItem)ConsolesTree.Items[0];
            root.Items.Clear();
            foreach (var curSystem in _systemLister.Systems())
            {
                Label newLabel = new Label();
                newLabel.Content = curSystem.ShortName;
                newLabel.MouseLeftButtonUp += new MouseButtonEventHandler(ClickSystem);

                root.Items.Add(newLabel);
            }

        }
    }
}
