using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MahloService.Settings;
using Serilog;

namespace MahloService.Utilities
{
  internal class OpcServerController : IOpcServerController
  {
    const string OpcServerWindowCaption = "mahlo 10A OPC-Server";

    private const int GW_HWNDNEXT = 2;
    private const int WM_GETTEXT = 13;
    private const int WM_GETTEXTLENGTH = 14;
    private const int WM_CLOSE = 16;

    private readonly IOpcSettings opcSettings;
    private readonly ILogger logger;
    private readonly string processName;

    private Process process;
    private int opcWinHandle;
    private int opcInstanceHandle;

    public OpcServerController(IOpcSettings opcSettings, ILogger logger)
    {
      this.opcSettings = opcSettings;
      this.logger = logger;
      this.processName = Path.GetFileNameWithoutExtension(opcSettings.OpcServerPath);
    }

    public void Start()
    {
      // If we're just starting, we don't have the OpcServer process information 
      // so get the information if it is running
      if (this.process == null)
      {
        Process[] processes = Process.GetProcessesByName(this.processName);
        if (processes.Length > 1)
        {
          // More than one is running, stop them all
          Parallel.ForEach(processes, p =>
          {
            try
            {
              if (!p.HasExited)
              {
                this.Stop(p);
                p.WaitForExit();
              }
            }
            catch (Exception)
            {
            }
          });
        }
        else
        {
          this.process = processes.SingleOrDefault();
          if (this.process != null)
          {
            try
            {          
              // if process is exiting, give it a chance to finish
              if (this.process.WaitForExit(5000))
              {
                // Yep, it exited.
                this.process = null;
              }
            }
            catch (Exception)
            {
              this.process = null;
            }
          }
        }
      }

      if (this.process != null)
      {
        // The process is already running
        return;
      }

      // We need to start it.
      ProcessStartInfo startInfo = new ProcessStartInfo(this.opcSettings.OpcServerPath)
      {
        WindowStyle = ProcessWindowStyle.Minimized
      };

      this.process = Process.Start(startInfo);
      this.process.WaitForInputIdle();
    }


    private async Task xStart()
    {
      try
      {
        if (this.opcInstanceHandle != 0)
        {
          int beginTick = Environment.TickCount;
          while ((IsAppRunning() && Environment.TickCount - beginTick <= 5000))
          {
            //In case application is shutting down, give it time to end
            await Task.Delay(200);
          };
        }

        //If it is still running, exit
        if ((IsAppRunning()))
        {
          return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo(this.opcSettings.OpcServerPath)
        {
          WindowStyle = ProcessWindowStyle.Minimized
        };

        Process process = Process.Start(startInfo);
        this.opcInstanceHandle = process.Id;
        if (this.opcInstanceHandle != 0)
        {
          process.WaitForInputIdle();
          this.opcWinHandle = GetWinHandle(this.opcInstanceHandle);
        }
      }
      catch (System.Exception ex)
      {
        this.logger.Error("LaunchOpcServer", ex.Message);
        return;
      }
    }

    public void Stop()
    {
      this.Stop(this.process);
    }

    private void Stop(Process process)
    {
      NativeMethods.PostMessage(process.MainWindowHandle, WM_CLOSE, 0, 0);
    }

    //private bool IsAppRunning(string ClassName, string Caption)
    private bool IsAppRunning()
    {
      var processes = Process.GetProcessesByName(this.processName);
      return processes.Any();
      //Caption = OpcServerWindowCaption;
      //if (ClassName == string.Empty)
      //{
      //  ClassName = null;
      //}

      //int hwnd = NativeMethods.FindWindow(ClassName, Caption);
      //return hwnd != 0;
    }

    private static string GetWinText(int hwnd)
    {
      IntPtr windowHandle = new IntPtr(hwnd);

      // Allocate correct string length first
      int length = (int)NativeMethods.SendMessage(windowHandle, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
      StringBuilder sb = new StringBuilder(length + 1);
      NativeMethods.SendMessage(windowHandle, WM_GETTEXT, (IntPtr)sb.Capacity, sb);
      return sb.ToString();
    }

    private int GetWinHandle(int hInstance)
    {
      int result = 0;
      int hwnd = 0;

      // Grab the first window handle that Windows finds:
      hwnd = NativeMethods.FindWindow(null, null);

      // Loop until you find a match or there are no more window handles:
      while (hwnd != 0)
      {
        // Check if no parent for this window
        if (NativeMethods.GetParent(hwnd) == 0)
        {
          // Check for PID match
          if (hInstance == ProcIDFromWnd(hwnd))
          {
            if (GetWinText(hwnd).ToLower() == this.opcSettings.OpcServerName.ToLower())
            {
              // Return found handle
              result = hwnd;
              // Exit search loop
              break;
            }
          }
        }

        // Get the next window handle
        hwnd = NativeMethods.GetWindow(hwnd, GW_HWNDNEXT);
      };

      return result;
    }

    private int ProcIDFromWnd(int hwnd)
    {
        int idProc = 0;

        // Get PID for this HWnd
        NativeMethods.GetWindowThreadProcessId(hwnd, ref idProc);

        // Return PID
        return idProc;
    }

    private static class NativeMethods
    {
      [DllImport("user32.dll", EntryPoint = "FindWindowW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
      extern public static int FindWindow(string lpClassName, string lpWindowName);

      [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
      extern public static int GetParent(int hwnd);

      [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
      extern public static int GetWindow(int hwnd, int wCmd);

      [DllImport("user32.dll", EntryPoint = "GetWindowTextA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
      extern public static int GetWindowText(int hwnd, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpString, int cch);

      [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
      extern public static int GetWindowThreadProcessId(int hwnd, ref int lpdwprocessid);

      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      public extern static IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [Out] StringBuilder lParam);

      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      public extern static IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

      [DllImport("user32.dll", EntryPoint = "PostMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
      extern public static int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
    }
  }
}
