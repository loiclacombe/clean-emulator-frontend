
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CleanEmulatorFrontend.Engine.Data;
using CleanEmulatorFrontend.Engine.Launchers;
using CleanEmulatorFrontend.Engine.Listers;
using GamesData.DatData;
using Microsoft.Practices.Unity;
using log4net;
using log4net.Repository.Hierarchy;


namespace CleanEmulatorFrontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ILog _logger = LogManager.GetLogger(typeof(MainWindow));
        private List<Game> _games=new List<Game>();
        private AppLoader _appLoader;
        private IEnumerable<SystemGroup> _systemGroups;

        public MainWindow()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<AppLoader>();
            _appLoader = container.Resolve<AppLoader>();
            InitializeComponent();
            InitTree();
            WindowState = WindowState.Maximized;
        }

        private void InitTree()
        {
            _systemsTree.Items.Clear();
            try
            {
                _systemGroups = _appLoader.LoadDats(); 
            }catch(Exception e)
            {
                _logger.Error(e,e);
            }

            _systemsTree.ItemsSource = _systemGroups;
        }


        private void _systemsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var value = e.NewValue;
            var games = new List<Game>();

            if (value.GetType() == typeof(EmulatedSystem))
            {
                var emulatedSystem = value as EmulatedSystem;
                games.AddRange(emulatedSystem.Games);
            }
            else if (value.GetType() == typeof(SystemGroup))
            {
                var systemGroup = value as SystemGroup;
                systemGroup.Systems.ToList().ForEach(sg => games.AddRange(sg.Games));
            }
            _gamesGrid.ItemsSource = games;
        }

        private void _gamesGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element != null && element is FrameworkElement && ((FrameworkElement)element).Parent is DataGridCell)
            {
                var grid = sender as DataGrid;
                LaunchSelectedGame(grid);
            }
        }

        private static void LaunchSelectedGame(DataGrid grid)
        {
            if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
            {
                var game = grid.SelectedItem as Game?;
                if (game.HasValue)
                {
                    new Higan().Run(game.Value);
                }
            }
        }

        private void _gamesGrid_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
    
                var grid = e.Source as DataGrid;
            LaunchSelectedGame(grid);
        }


    }
}
