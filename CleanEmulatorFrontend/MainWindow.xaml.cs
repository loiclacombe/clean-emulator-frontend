using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Castle.Core.Internal;
using GamesData;
using log4net;
using ReactiveUI;

namespace CleanEmulatorFrontend
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof (MainWindow));

        private readonly AppLoader _appLoader;
        private readonly ReactiveList<Game> _displayed = new ReactiveList<Game>();

        private IEnumerable<Game> _all = new List<Game>();
        private SystemsCache _systemsCache;

        public MainWindow(AppLoader appLoader)
        {
            _appLoader = appLoader;

            InitializeComponent();
        }

        public void InitializeContent()
        {
            _systemsCache = _appLoader.LoadLibraries();
            SystemsTree.ItemsSource = _systemsCache.Groups;
            GamesGrid.ItemsSource = _displayed;
            SelectDefaultSystem();
            InitEvents();

            WindowState = WindowState.Maximized;
        }

        private void InitEvents()
        {
            SystemSelectionEvents();
            GameLaunchEvents();

            this.Events()
                .KeyUp
                .Where(IsCtrlAndK)
                .Subscribe(o =>
                           {
                               SearchGameBlock.Text = string.Empty;
                               SearchGameBlock.Focus();
                           });

            SearchGameBlock
                .Events()
                .KeyUp
                .Where(k => k.Key == Key.Enter)
                .Subscribe(e => FilterGames(SearchGameBlock.Text));
        }

        private void GameLaunchEvents()
        {
            GamesGrid.Events()
                .KeyUp
                .Where(k => k.Key == Key.Enter)
                .Subscribe(o => LaunchSelectedGame());

            //prevent movement to next line on press enter
            GamesGrid.Events()
                .PreviewKeyDown
                .Where(k => k.Key == Key.Enter)
                .Subscribe(e => e.Handled = true);

            GamesGrid.Events()
                .MouseDoubleClick
                .Select(e => e.MouseDevice.DirectlyOver)
                .Where(e => e is FrameworkElement && ((FrameworkElement) e).Parent is DataGridCell)
                .Subscribe(e => LaunchSelectedGame());
        }

        private void FilterGames(string text)
        {
            string[] words = text.ToLower().Split(' ');
            _displayed.Clear();
            _displayed.AddRange(_all.Where(g => AllWordsAreContainedIn(g, words)));
        }

        private void SystemSelectionEvents()
        {
            IObservable<object> systemChanged = SystemsTree
                .Events()
                .SelectedItemChanged
                .Select(s => s.NewValue);

            systemChanged
                .Where(v => v.GetType() == typeof (EmulatedSystem))
                .Select(v => v as EmulatedSystem)
                .Subscribe(es =>
                           {
                               _displayed.Clear();
                               _all = _systemsCache.FilterBySystem(es);
                               _displayed.AddRange(_all);
                           });

            systemChanged
                .Where(v => v.GetType() == typeof (SystemGroup))
                .Select(v => v as SystemGroup)
                .Subscribe(sg =>
                           {
                               _displayed.Clear();
                               _all = _systemsCache.FilterBySystemGroup(sg);
                               _displayed.AddRange(_all);
                           });
        }

        private void SelectDefaultSystem()
        {
            SystemsTree.SetSelectedItem("Consoles",
                o => o.GetType() == typeof (SystemGroup) ? ((SystemGroup) o).Description : "toto");
        }


        private void LaunchSelectedGame()
        {
            if (GamesGrid.SelectedItems.Count == 1)
            {
                var game = GamesGrid.SelectedItem as Game;
                if (game == null) return;
                ILauncher launcher = game.System.Emulator.Launcher;
                Process process = launcher.StartGame(game);
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    var errors = process.StandardError.ReadToEnd();
                    if (!errors.IsNullOrEmpty())
                    {
                        ErrorDialog.DisplayError(process.StartInfo + "\n" + errors);
                    }
                }
            }
        }

        private static bool AllWordsAreContainedIn(Game game, IEnumerable<string> words)
        {
            return words.All(word => game.Description.ToLower().Contains(word));
        }

        public static void SetSelectedItem(ref TreeView control, object item)
        {
            try
            {
                DependencyObject dObject = control
                    .ItemContainerGenerator
                    .ContainerFromItem(item);

                //uncomment the following line if UI updates are unnecessary
                ((TreeViewItem) dObject).IsSelected = true;

                MethodInfo selectMethod =
                    typeof (TreeViewItem).GetMethod("Select",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                selectMethod.Invoke(dObject, new object[] {true});
            }
            catch (Exception)
            {
            }
        }

        private static bool IsCtrlAndK(KeyEventArgs e)
        {
            return e.Key == Key.K && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        }
    }
}