using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace AssertAutoIgnore
{
    public class Program
    {
        private static volatile bool _isRunning = true;
        private static readonly WindowDialog[] DialogsToHandle = new[]
        {
            new WindowDialog("Always Ignore?", 0),
            new WindowDialog("RGL CONTENT WARNING", 0),
            new WindowDialog("RGL CONTENT ERROR", 0),
            new WindowDialog("RGL WARNING", 0),
            new WindowDialog("Safe Mode", 1),
            new WindowDialog("*_*", 1),
            new WindowDialog("ASSERT", 2),
            new WindowDialog("SAFE ASSERT", 2)
        };

        private static void Main()
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                Log("Ctrl+C pressed. Exiting...");
                _isRunning = false;
                e.Cancel = true;
            };

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (_isRunning)
            {
                bool handled = TryHandleDialogs();

                if (handled)
                {
                    stopwatch.Restart();
                }
                else if (stopwatch.Elapsed.TotalSeconds > 5)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private static bool TryHandleDialogs()
        {
            foreach (var dialog in DialogsToHandle)
            {
                if (IsTargetDialogVisible(dialog, out IntPtr hWnd))
                {
                    Log($"Found dialog: \"{dialog.Title}\"");
                    HandleDialogButtons(hWnd, dialog.ButtonIndex);
                    return true;
                }
            }

            return false;
        }

        private static bool IsTargetDialogVisible(WindowDialog dialog, out IntPtr hWnd)
        {
            hWnd = NativeMethods.FindWindow(null, dialog.Title);
            return hWnd != IntPtr.Zero && NativeMethods.IsWindowVisible(hWnd);
        }

        private static void HandleDialogButtons(IntPtr hWnd, int targetButtonIndex)
        {
            int currentIndex = 0;

            NativeMethods.EnumChildWindows(hWnd, (childHwnd, lParam) =>
            {
                var className = new StringBuilder(256);
                NativeMethods.GetClassName(childHwnd, className, className.Capacity);

                if (className.ToString().Contains("Button"))
                {
                    if (currentIndex == targetButtonIndex)
                    {
                        Log($"Clicking button index {targetButtonIndex}...");
                        NativeMethods.SendMessage(childHwnd, NativeMethods.WindowsMessageButtonClick, IntPtr.Zero, IntPtr.Zero);
                        return false;
                    }

                    currentIndex++;
                }

                return true;
            }, IntPtr.Zero);
        }

        private static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
    }
}
