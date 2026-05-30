using System;
using System.Runtime.InteropServices;

namespace WinUI.SingleInstanced
{
    internal static class Win32
    {
        public static class Kernel32
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string? lpName);

            [DllImport("kernel32.dll")]
            public static extern bool SetEvent(IntPtr hEvent);
        }

        public static class Ole32
        {

            [DllImport("ole32.dll")]
            public static extern uint CoWaitForMultipleObjects(uint dwFlags, uint dwMilliseconds, ulong nHandles, IntPtr[] pHandles, out uint dwIndex);
        }

        public static class User32
        {
            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }

    }
}
