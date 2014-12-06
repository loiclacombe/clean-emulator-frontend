using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MainWindow));

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
            var doubleClickedOnGame = GamesGrid.Events()
                .MouseDoubleClick
                .Select(e => e.MouseDevice.DirectlyOver)
                .Where(e => e is FrameworkElement
                            && ((FrameworkElement)e).Parent is DataGridCell)
                .Where(e => OnlyOneGameIsSelected())
                .Select(SelectedGame);
            var pressedEnterToLaunchGame = GamesGrid.Events()
                .KeyUp
                .Where(k => k.Key == Key.Enter)
                .Where(e => OnlyOneGameIsSelected())
                .Select(SelectedGame);

            pressedEnterToLaunchGame
                .Merge(doubleClickedOnGame)
                .Subscribe(LaunchSelectedGame);

            //prevent movement to next line on press enter
            GamesGrid.Events()
                .PreviewKeyDown
                .Where(k => k.Key == Key.Enter)
                .Subscribe(e => e.Handled = true);
        }

        private Game SelectedGame(Object anything)
        {
            return GamesGrid.SelectedItem as Game;
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


        private void LaunchSelectedGame(Game game)
        {
            if (game == null) return;
            game.Start()
                .Where(p => p.ExitCode != 0
                            && string.IsNullOrEmpty(p.ErrorMessage))
                .Subscribe(
                    process =>
                        ErrorDialog.DisplayError(process.StartInfo + "\n" + process.ErrorMessage));
        }

        private bool OnlyOneGameIsSelected()
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

    public class ExitedProcess
    {
        private readonly string _errorMessage;
        private readonly int _exitCode;
        private readonly ProcessStartInfo _startInfo;

        public ExitedProcess(Process process)
        {
            _exitCode = process.ExitCode;
            _errorMessage = process.StandardError.ReadToEnd();
            _startInfo = process.StartInfo;
        }

        public ProcessStartInfo StartInfo
        {
            get { return _startInfo; }
        }

        public int ExitCode
        {
            get { return _exitCode; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }
    }
}