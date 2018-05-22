using Microsoft.Win32.SafeHandles;
using System;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

public sealed class SingleInstance : IDisposable
{
  private EventWaitHandle eventWaitHandle;

  /// <summary>
  /// Initializes a new instance of the SingleGLobalInstance class.
  /// </summary>
  /// <param name="wparam">A value to be passed to the first instance if an instance is already running.
  public SingleInstance(string instanceName, int wparam = 0)
  {
    WM_SHOWFIRSTINSTANCE = WinApi.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}", instanceName);
    bool createdNew = InitEvent(instanceName);
    if (!createdNew)
    {
      WinApi.PostMessage(
          (IntPtr)WinApi.HWND_BROADCAST,
          WM_SHOWFIRSTINSTANCE,
          new IntPtr(wparam),
          IntPtr.Zero);

      Assembly appAssembly = Assembly.GetEntryAssembly();
      string appName = ((AssemblyProductAttribute)appAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false).GetValue(0)).Product;
      throw new SingleInstance.Exception(string.Format("Another instance of '{0}' is running.", appName));
    }
  }

  public static int WM_SHOWFIRSTINSTANCE { get; private set; }

  public void Dispose()
  {
    this.eventWaitHandle.Dispose();
  }

  private bool InitEvent(string instanceName)
  {
    this.eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, instanceName, out bool createdNew);

    var allowEveryoneRule = new EventWaitHandleAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), EventWaitHandleRights.FullControl, AccessControlType.Allow);
    var securitySettings = new EventWaitHandleSecurity();
    securitySettings.AddAccessRule(allowEveryoneRule);
    this.eventWaitHandle.SetAccessControl(securitySettings);
    return createdNew;
  }

  [Serializable]
  public class Exception : ApplicationException
  {
    public Exception(string message)
      : base(message)
    {
    }
  }
}
