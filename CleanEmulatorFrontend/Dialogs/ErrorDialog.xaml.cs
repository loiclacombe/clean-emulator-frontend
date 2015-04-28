using System;
using System.Windows;

namespace CleanEmulatorFrontend.Dialogs
{
    /// <summary>
    ///     Interaction logic for ErrorDialog.xaml
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
            var message = e.Message + "\n" + e.StackTrace;
            if (e.InnerException != null)
            {
                message += "\n" + e.InnerException.Message + "\n" + e.InnerException.StackTrace;
            }
            new ErrorDialog("Startup error", message);
        }

        public static void DisplayError(string error)
        {
            new ErrorDialog("Error", error);
        }
    }
}