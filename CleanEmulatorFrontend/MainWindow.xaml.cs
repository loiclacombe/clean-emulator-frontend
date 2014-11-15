using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CleanEmulatorFrontend.Controllers;
using CleanEmulatorFrontend.Engine.Data;
using CleanEmulatorFrontend.Engine.Launchers;
using GamesData.DatData;
using log4net;

namespace CleanEmulatorFrontend
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof (MainWindow));

        private readonly AppLoader _appLoader;
        private LaunchController _launchController;
        private IEnumerable<SystemGroup> _systemGroups;

        public MainWindow(AppLoader appLoader, LaunchController launchController)
        {
            _appLoader = appLoader;
            _launchController = launchController;

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
            }
            catch (Exception e)
            {
                _logger.Error(e, e);
            }

            _systemsTree.ItemsSource = _systemGroups;
        }


        private void _systemsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            object value = e.NewValue;
            var games = new List<Game>();

            if (value.GetType() == typeof (EmulatedSystem))
            {
                var emulatedSystem = value as EmulatedSystem;
                games.AddRange(emulatedSystem.Games);
            }
            else if (value.GetType() == typeof (SystemGroup))
            {
                var systemGroup = value as SystemGroup;
                systemGroup.Systems.ToList().ForEach(sg => games.AddRange(sg.Games));
            }
            _gamesGrid.ItemsSource = games;
        }

        private void _gamesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element != null && element is FrameworkElement && ((FrameworkElement) element).Parent is DataGridCell)
            {
                var grid = sender as DataGrid;
                LaunchSelectedGame(grid);
            }
        }

        private void LaunchSelectedGame(DataGrid grid)
        {
            if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
            {
                var game = grid.SelectedItem as Game?;
                if (game.HasValue)
                {
                    IEmulator launcher = FindLauncher();
                    var process=launcher.StartGame(game.Value);
                    process.WaitForExit();
                }
            }
        }

        private IEmulator FindLauncher()
        {
            return new Higan();
        }


        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);


        public void FullScreen(Process process)
        {
            Thread.Sleep(5000);
            _logger.DebugFormat("Process handle : {0}", process.MainWindowHandle);
            SetForegroundWindow(process.MainWindowHandle);
            SetFocus(process.MainWindowHandle);
        }
    }
}