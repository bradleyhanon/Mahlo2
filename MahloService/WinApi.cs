using System;
using System.Globalization;
using System.Runtime.InteropServices;

public static class WinApi
{
  public static int RegisterWindowMessage(string format, params object[] args)
  {
    string message = string.Format(CultureInfo.InvariantCulture, format, args);
    return NativeMethods.RegisterWindowMessage(message);
  }

  public static void ShowToFront(IntPtr window)
  {
    NativeMethods.ShowWindow(window, NativeMethods.SW_SHOWNORMAL);
    NativeMethods.SetForegroundWindow(window);
  }

  internal static void PostBroadcastMessage(int msg, IntPtr lParam, IntPtr wParam)
  {
    NativeMethods.PostMessage(new IntPtr(NativeMethods.HWND_BROADCAST), msg, lParam, wParam);
  }

  private static class NativeMethods
  {
    public const int HWND_BROADCAST = 0xffff;
    public const int SW_SHOWNORMAL = 1;

    [DllImport("user32", CharSet = CharSet.Unicode)]
    public static extern int RegisterWindowMessage(string message);

    [DllImport("user32")]
    public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
  }
}

