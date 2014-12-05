using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using GamesData.DatData;
using WindowsInput;
using WindowsInput.Native;
using log4net;

namespace CleanEmulatorFrontend.Engine.Launchers
{
    public class Higan
    {
        private static ILog _logger = LogManager.GetLogger(typeof(Higan));
        public void Run(Game game)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = ConfigurationManager.AppSettings["emulators.higan.path"];
            process.StartInfo.Arguments = "\"" + game.LaunchPath + "\" fullscreen";
            process.Start();
            FullScreen(process);
            process.WaitForExit();
        }

        const ushort WM_SYSKEYDOWN = 260;
        private const ushort WM_SYSKEYUP = 261;
        private const ushort WM_CHAR = 258;
        private const ushort WM_KEYDOWN = 256;
        private const ushort WM_KEYUP = 257;


        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);


        [DllImport("user32.dll")]
        private static extern IntPtr FlashWindow(
            IntPtr hwnd,
            IntPtr bInvert
        );

        public void FullScreen(Process process)
        {
            Thread.Sleep(5000);
            _logger.DebugFormat("Process handle : {0}", process.MainWindowHandle);
            SetForegroundWindow(process.MainWindowHandle);
            SetFocus(process.MainWindowHandle);

            var inputSimulator = new InputSimulator();
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F11);

            const int KEYEVENTF_KEYUP = 0x0002;

            keybd_event((byte) VirtualKeyCode.F11, 0, 0, IntPtr.Zero);
            keybd_event((byte) VirtualKeyCode.F11, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }


    }

}
