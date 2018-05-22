using System;
using System.Threading;
using System.Runtime.InteropServices;


namespace MahloService.Utilities
{
  /// <summary> 
  /// The event that occurred. 
  /// </summary> 
  public enum ConsoleEvent
  {
    CTRL_C = 0,             // From wincom.h 
    CTRL_BREAK = 1,
    CTRL_CLOSE = 2,
    CTRL_LOGOFF = 5,
    CTRL_SHUTDOWN = 6
  }

  /// <summary> 
  /// Class to catch console control events (ie CTRL-C) in C#. 
  /// Calls SetConsoleCtrlHandler() in Win32 API 
  /// </summary> 
  public class ConsoleCtrl : IDisposable
  {
    /// <summary> 
    /// Handler to be called when a console event occurs. 
    /// </summary> 
    public delegate bool ControlEventHandler(ConsoleEvent consoleEvent);


    /// <summary> 
    /// Event fired when a console event occurs 
    /// </summary> 
    public event EventHandler<ConsoleEventArgs> ControlEvent;

    ControlEventHandler eventHandler;

    /// <summary> 
    /// Create a new instance. 
    /// </summary> 
    public ConsoleCtrl()
    {
      // save this to a private var so the GC doesn't collect it... 
      eventHandler = new ControlEventHandler(Handler);
      NativeMethods.SetConsoleCtrlHandler(eventHandler, true);
    }


    ~ConsoleCtrl()
    {
      Dispose(false);
    }


    public void Dispose()
    {
      Dispose(true);
    }


    void Dispose(bool disposing)
    {
      if (eventHandler != null)
      {
        NativeMethods.SetConsoleCtrlHandler(eventHandler, false);
        eventHandler = null;
      }
    }


    private bool Handler(ConsoleEvent consoleEvent)
    {
      var args = new ConsoleEventArgs(consoleEvent);
      this.ControlEvent?.Invoke(this, args);
      return args.Result;
    }

    private static class NativeMethods
    {
      [DllImport("kernel32.dll")]
      public static extern bool SetConsoleCtrlHandler(ControlEventHandler e, bool add);
    }
  }
}
