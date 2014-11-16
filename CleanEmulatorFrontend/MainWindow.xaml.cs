using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Castle.Core.Internal;
using GamesData;
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
        private SystemsCache _systemsCache;
        private IEnumerable<Game> _filteredGames;
        private IEnumerable<Game> _nonFilteredGames;

        public MainWindow(AppLoader appLoader)
        {
            _appLoader = appLoader;

            InitializeComponent();
        }

        public void InitializeContent()
        {
            try
            {
                _systemsCache = _appLoader.LoadLibraries();
                SystemsTree.ItemsSource = _systemsCache.Groups;
                SystemsTree.SetSelectedItem("Consoles", o=> o.GetType() == typeof(SystemGroup)? ((SystemGroup)o).Description:"toto");
            }
            catch (Exception e)
            {
                _logger.Error(e, e);
                var message = e.Message+"\n"+e.StackTrace;
                if (e.InnerException != null)
                {
                    message +=  "\n"+e.InnerException.Message + "\n" + e.InnerException.StackTrace;
                }
                new ErrorDialog("Startup error", message);
            }
            WindowState = WindowState.Maximized;
        }


        private void _systemsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            object value = e.NewValue;
            IEnumerable<Game> games = new List<Game>();

            if (value.GetType() == typeof (EmulatedSystem))
            {
                var emulatedSystem = value as EmulatedSystem;
                games=_systemsCache.FilterBySystem(emulatedSystem);
            }
            else if (value.GetType() == typeof (SystemGroup))
            {
                var systemGroup = value as SystemGroup;
                games = _systemsCache.FilterBySystemGroup(systemGroup);
            }
            GamesGrid.ItemsSource =_nonFilteredGames = games;
        }



        private void _gamesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element is FrameworkElement && ((FrameworkElement) element).Parent is DataGridCell)
            {
                LaunchSelectedGame();
            }
        }

        private void LaunchSelectedGame()
        {
            if (GamesGrid.SelectedItems.Count == 1)
            {
                var game = GamesGrid.SelectedItem as Game;
                if (game == null) return;
                ILauncher launcher = FindLauncher(game);
                Process process = launcher.StartGame(game);
                process.WaitForExit();
            }
        }

        private ILauncher FindLauncher(Game game)
        {
            return game.System.Emulator.Launcher;
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

        private void SearchGameBlock_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilterGames();
            }
            else if (e.Key == Key.Down)
            {
                GamesGrid.SelectedIndex = 0;
            }
        }

        private void FilterGames()
        {
            if (SearchGameBlock.Text.IsNullOrEmpty())
            {
                GamesGrid.ItemsSource = _filteredGames = _nonFilteredGames;
            }
            else
            {
               GamesGrid.ItemsSource = _filteredGames = Filter(SearchGameBlock.Text);
            }
        }


        public IEnumerable<Game> Filter( string search)
        {
            var words = search.ToLower().Split(' ');
            return _nonFilteredGames.Where(g => AllWordsAreContainedIn(g, words));
        }

        private static bool AllWordsAreContainedIn(Game game, IEnumerable<string> words)
        {
            return words.All(word => game.Description.ToLower().Contains(word));
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (IsCtrlAndK(e))
            {
                SearchGameBlock.Text = string.Empty;
                SearchGameBlock.Focus();
            }
        }

        public static void SetSelectedItem(ref TreeView control, object item)
        {
            try
            {
                DependencyObject dObject = control
                    .ItemContainerGenerator
                    .ContainerFromItem(item);

                //uncomment the following line if UI updates are unnecessary
                ((TreeViewItem)dObject).IsSelected = true;                

                MethodInfo selectMethod =
                   typeof(TreeViewItem).GetMethod("Select",
                   BindingFlags.NonPublic | BindingFlags.Instance);

                selectMethod.Invoke(dObject, new object[] { true });
            }
            catch { }
        }

        private static bool IsCtrlAndK(KeyEventArgs e)
        {
            return e.Key == Key.K && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        }

        private void GamesGrid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private void GamesGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LaunchSelectedGame();
                GamesGrid.SelectedIndex--;
                e.Handled = true;
            }
        }
    }
}