using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CleanEmulatorFrontend
{
    /// <summary>
    /// Interaction logic for ErrorDialog.xaml
    /// </summary>
    public partial class ErrorDialog : Window
    {
        public ErrorDialog(string title, string message)
        {
            InitializeComponent();
            Message = message;
            ShowDialog();
        }

        public string Message
        {
            set { MessageBlock.Text = value; }
            get { return MessageBlock.Text; }
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static void DisplayException(Exception e)
        {
            string message = e.Message + "\n" + e.StackTrace;
            if (e.InnerException != null)
            {
                message += "\n" + e.InnerException.Message + "\n" + e.InnerException.StackTrace;
            }
            new ErrorDialog("Startup error", message);
        }
    }
}
