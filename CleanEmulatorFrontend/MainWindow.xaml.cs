using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CleanEmulatorFrontend.Dialogs;
using GamesData;
using Launchers;
using log4net;
using ReactiveUI;
// ReSharper disable PossibleMultipleEnumeration
namespace CleanEmulatorFrontend
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MainWindow));

        private readonly Random _random;
        private readonly Func<ConfigureEmulators> _configureEmulators;
        private readonly FailsafeCacheLoader _failsafeCacheLoader;

        private IEnumerable<Game> _all = new List<Game>();
        private LoadedSystems _loadedSystems;
        private readonly ReactiveList<Game> _displayed = new ReactiveList<Game>();


        public MainWindow(FailsafeCacheLoader failsafeCacheLoader, Random random,
            Func<ConfigureEmulators> configureEmulators)
        {
            _failsafeCacheLoader = failsafeCacheLoader;
            _random = random;
            _configureEmulators = configureEmulators;
            _failsafeCacheLoader = failsafeCacheLoader;

            InitializeComponent();
        }

        public void InitializeContent()
        {
            _failsafeCacheLoader.LoadLibraries();
            SelectDefaultSystem();
            InitTrees();
            InitEvents();

            WindowState = WindowState.Maximized;
        }

        private void InitTrees()
        {
            _loadedSystems = _failsafeCacheLoader.LoadLibraries();
            SystemsTree.ItemsSource = _loadedSystems.Groups;
            GamesGrid.ItemsSource = _displayed;
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

            SearchGameBlock.Events()
                .KeyUp
                .Where(k => k.Key == Key.Enter)
                .Select(k => SearchGameBlock.Text)
                .Subscribe(FilterGames);

            RefreshCacheButton.Events()
                .PreviewMouseDown
                .Subscribe(e => _failsafeCacheLoader.ForceLoadFromDats());

            ConfigureButton.Events()
                .PreviewMouseDown
                .Subscribe(e =>
                           {
                               var configureEmulators = _configureEmulators();
                               configureEmulators.ShowDialog();
                           });

        }



        private void GameLaunchEvents()
        {
            IObservable<Game> doubleClickedOnGame = GamesGrid.Events()
                .MouseDoubleClick
                .Select(e => e.MouseDevice.DirectlyOver)
                .Where(e => e is FrameworkElement
                            && ((FrameworkElement) e).Parent is DataGridCell)
                .Where(OnlyOneGameIsSelected)
                .Select(SelectedGame);
            IObservable<Game> pressedEnterToLaunchGame = GamesGrid.Events()
                .KeyUp
                .Where(k => k.Key == Key.Enter)
                .Where(OnlyOneGameIsSelected)
                .Select(SelectedGame);

            IObservable<Game> clickedRandomGameLaunch = RandomGameButton.Events()
                .PreviewMouseLeftButtonDown
                .Select(PickRandomGame);

            pressedEnterToLaunchGame
                .Merge(doubleClickedOnGame)
                .Merge(clickedRandomGameLaunch)
                .Subscribe(LaunchSelectedGame);

            //prevent movement to next line on press enter
            GamesGrid.Events()
                .PreviewKeyDown
                .Where(k => k.Key == Key.Enter)
                .Subscribe(e => e.Handled = true);
        }

        private Game PickRandomGame(MouseButtonEventArgs arg)
        {
            int randomGameIndex = _random.Next(_displayed.Count);
            return _displayed[randomGameIndex];
        }

        internal Game SelectedGame(Object anything)
        {
            return GamesGrid.SelectedItem as Game;
        }

        internal void FilterGames(string text)
        {
            string[] words = text.ToLower().Split(' ');
            _displayed.Clear();
            var filtered = _all.Where(g => AllWordsAreContainedIn(g, words)).ToList();
            if (filtered.Any())
                filtered.ForEach(_displayed.Add);
        }

        internal void SystemSelectionEvents()
        {
            IObservable<object> selectedSystemChanged = SystemsTree
                .Events()
                .SelectedItemChanged
                .Select(s => s.NewValue);

            var selectedGamesForSystem = selectedSystemChanged
                .OfType<EmulatedSystem>()
                .Select(_loadedSystems.FilterBy);

            var selectedGamesForGroup = selectedSystemChanged
                .OfType<SystemNode>()
                .Select(_loadedSystems.FilterBy);

            selectedGamesForGroup
                .Merge(selectedGamesForSystem)
                .Subscribe(UpdateGamesGrid);
        }

        private void UpdateGamesGrid(IEnumerable<Game> games)
        {
            _displayed.Clear();

            _all = games;
            if (games.Any())
            {
                _displayed.AddRange(_all);
            }
        }

        internal void SelectDefaultSystem()
        {
            SystemsTree.SetSelectedItem("Consoles",
                o => o.GetType() == typeof (SystemGroup) ? ((SystemGroup) o).Description : "zzzzzzzz");
        }


        internal void LaunchSelectedGame(Game game)
        {
            if (game == null) return;
            game.Start()
                .Where(p => p.ExitCode != 0
                            && string.IsNullOrEmpty(p.ErrorMessage))
                .Subscribe(
                    process =>
                        ErrorDialog.DisplayError(process.StartInfo + "\n" + process.ErrorMessage));
        }

        internal bool OnlyOneGameIsSelected(Object o)
        {
            return GamesGrid.SelectedItems.Count == 1;
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
            catch (Exception e)
            {
                Logger.Debug(e, e);
            }
        }

        private static bool IsCtrlAndK(KeyEventArgs e)
        {
            return e.Key == Key.K && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        }
    }

    public static class GameLaunchExt
    {
        public static IObservable<ExitedProcess> Start(this Game game)
        {
            Process process = game.System.Emulator.Launcher.StartGame(game);
            return Observable.FromEventPattern(
                ev => process.Exited += ev,
                ev => process.Exited -= ev)
                .Select(ev => ev.Sender as Process)
                .Select(p => new ExitedProcess(p));
        }
    }
}