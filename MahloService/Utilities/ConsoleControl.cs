using System;
using System.Runtime.InteropServices;


namespace MahloService.Utilities
{
  /// <summary> 
  /// The event that occurred. 
  /// </summary> 
  public enum ConsoleEvent
  {
    CtrlC = 0,             // From wincom.h 
    CtrlBreak = 1,
    CtrlClose = 2,
    CtrlLogoff = 5,
    CtrlShutdown = 6
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

    private ControlEventHandler eventHandler;

    /// <summary> 
    /// Create a new instance. 
    /// </summary> 
    public ConsoleCtrl()
    {
      // save this to a private var so the GC doesn't collect it... 
      this.eventHandler = new ControlEventHandler(this.Handler);
      NativeMethods.SetConsoleCtrlHandler(this.eventHandler, true);
    }


    ~ConsoleCtrl()
    {
      this.Dispose(false);
    }


    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.eventHandler != null)
      {
        NativeMethods.SetConsoleCtrlHandler(this.eventHandler, false);
        this.eventHandler = null;
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
