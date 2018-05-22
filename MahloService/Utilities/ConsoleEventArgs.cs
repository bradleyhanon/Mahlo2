using System;

namespace MahloService.Utilities
{
  public class ConsoleEventArgs : EventArgs
  {
    public ConsoleEventArgs(ConsoleEvent consoleEvent)
    {
      this.ConsoleEvent = consoleEvent;
    }

    public ConsoleEvent ConsoleEvent { get; private set; }

    public bool Result { get; set; }
  }
}

