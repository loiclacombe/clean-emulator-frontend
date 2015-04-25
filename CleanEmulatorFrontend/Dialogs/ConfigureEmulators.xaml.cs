using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GamesData;

namespace CleanEmulatorFrontend.Dialogs
{
    /// <summary>
    /// Interaction logic for ConfigureEmulators.xaml
    /// </summary>
    public partial class ConfigureEmulators : Window
    {
        private readonly SystemConfigRoot _systemConfigRoot;

        public ConfigureEmulators(SystemConfigRoot systemConfigRoot)
        {
            _systemConfigRoot = systemConfigRoot;
            InitializeComponent();
            LibrariesBox.Items.Clear();
            LibrariesBox.ItemsSource = Libraries;
        }


        public IEnumerable<Library> Libraries
        {
            get { return _systemConfigRoot.Library.OrderBy(l=> l.Name); }
        }
    }
}
