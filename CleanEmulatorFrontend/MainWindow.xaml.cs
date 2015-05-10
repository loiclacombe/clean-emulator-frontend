using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CleanEmulatorFrontend.Dialogs;
using CleanEmulatorFrontend.GamesData;
using CleanEmulatorFrontend.GUI;
using log4net;
using Launchers;
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
        private readonly Func<ConfigureEmulators> _configureEmulators;
        private readonly ReactiveList<Game> _displayed = new ReactiveList<Game>();
        private readonly Random _random;
        private readonly SystemsDataLoader _systemsDataLoader;
        private IEnumerable<Game> _all = new List<Game>();
        private LoadedSystems _loadedSystems;
        private ReactiveCommand<LoadedSystems> _forceRefreshTreesFromData;

        public MainWindow(SystemsDataLoader systemsDataLoader, Random random,
            Func<ConfigureEmulators> configureEmulators)
        {
            _systemsDataLoader = systemsDataLoader;
            _random = random;
            _configureEmulators = configureEmulators;

            InitializeComponent();
        }

        public void InitializeContent()
        {
            var refreshTreesFromData = ReactiveCommand.CreateAsyncTask(async _ =>  await _systemsDataLoader.LoadLibraries());
            _forceRefreshTreesFromData = ReactiveCommand.CreateAsyncTask<LoadedSystems>(async _ => await _systemsDataLoader.ForceLoadFromDats());


            refreshTreesFromData.ExecuteAsync()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(loadedSystems =>
                {
                    RefreshTree(loadedSystems);
                    SelectDefaultSystem();
                });
            refreshTreesFromData.ThrownExceptions.Subscribe(e => { Logger.Error(e); });
            _forceRefreshTreesFromData.ThrownExceptions.Subscribe(e => { Logger.Error(e); });

            WindowState = WindowState.Maximized;
        }

        private void RefreshTree(LoadedSystems loadedSystems)
        {
            _loadedSystems = loadedSystems;
            SystemsTree.ItemsSource = _loadedSystems.Groups;
            GamesGrid.ItemsSource = _displayed;
            InitEvents();
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
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(FilterGames);


            RefreshCacheButton.Events()
                .PreviewMouseDown
                .Subscribe(e =>
                {
                    _forceRefreshTreesFromData.ExecuteAsync()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(RefreshTree);
                });

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
            var doubleClickedOnGame = GamesGrid.Events()
                .MouseDoubleClick
                .Select(e => e.MouseDevice.DirectlyOver)
                .Where(e => e is FrameworkElement
                            && ((FrameworkElement) e).Parent is DataGridCell)
                .Where(OnlyOneGameIsSelected)
                .Select(SelectedGame);
            var pressedEnterToLaunchGame = GamesGrid.Events()
                .KeyUp
                .Where(k => k.Key == Key.Enter)
                .Where(OnlyOneGameIsSelected)
                .Select(SelectedGame);

            var clickedRandomGameLaunch = RandomGameButton.Events()
                .PreviewMouseLeftButtonDown
                .Select(PickRandomGame);

            pressedEnterToLaunchGame
                .Merge(doubleClickedOnGame)
                .Merge(clickedRandomGameLaunch)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(LaunchSelectedGame);

            //prevent movement to next line on press enter
            GamesGrid.Events()
                .PreviewKeyDown
                .Where(k => k.Key == Key.Enter)
                .Subscribe(e => e.Handled = true);
        }

        private Game PickRandomGame(MouseButtonEventArgs arg)
        {
            var randomGameIndex = _random.Next(_displayed.Count);
            return _displayed[randomGameIndex];
        }

        internal Game SelectedGame(object anything)
        {
            return GamesGrid.SelectedItem as Game;
        }

        internal void FilterGames(string text)
        {
            var words = text.ToLower().Split(' ');
            _displayed.Clear();
            var filtered = _all.Where(g => AllWordsAreContainedIn(g, words)).ToList();
            if (filtered.Any())
                filtered.ForEach(_displayed.Add);
        }

        internal void SystemSelectionEvents()
        {
            var selectedSystemChanged = SystemsTree
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
            SystemsTree.SetSelectedItem("All Systems",
                o => o.GetType() == typeof (SystemNode) ? ((SystemNode) o).Description : "zzzzzzzz");
        }

        internal void LaunchSelectedGame(Game game)
        {
            if (game == null) return;
            var startResult = game.Start();
            _loadedSystems.RecordGameLaunch(game);
            startResult
                .Where(p => p.ExitCode != 0
                            && string.IsNullOrEmpty(p.ErrorMessage))
                .Subscribe(
                    process =>
                        ErrorDialog.DisplayError(process.StartInfo + "\n" + process.ErrorMessage));
        }

        internal bool OnlyOneGameIsSelected(object o)
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
                var dObject = control
                    .ItemContainerGenerator
                    .ContainerFromItem(item);

                //uncomment the following line if UI updates are unnecessary
                ((TreeViewItem) dObject).IsSelected = true;

                var selectMethod =
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
            var process = game.System.Emulator.Launcher.StartGame(game);
            return Observable.FromEventPattern(
                ev => process.Exited += ev,
                ev => process.Exited -= ev)
                .Select(ev => ev.Sender as Process)
                .Select(p => new ExitedProcess(p));
        }
    }
}