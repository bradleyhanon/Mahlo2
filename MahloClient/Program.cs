using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahloService.Logic;
using MahloService.Models;
using MahloClient.AppSettings;
using MahloClient.Ipc;
using MahloClient.Logic;
using MahloClient.Views;
using SimpleInjector;
using SimpleInjector.Diagnostics;

namespace MahloClient
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      using (var container = InitializeContainer())
      {
        var notUsed = container.GetInstance<MahloIpcClient>().Start();
        Application.Run(container.GetInstance<FormBowAndSkew>());
      }
    }

    private static Container InitializeContainer()
    {
      // This call sets the WindowsFormsSynchronizationContext.Current
      using (new Control()) { };

      Container container = new Container();
      container.RegisterSingleton<IAppInfo, AppInfo>();

      var registration = Lifestyle.Transient.CreateRegistration<MainForm>(container);
      registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Done by system");

      container.RegisterSingleton<IMahloIpcClient, MahloIpcClient>();
      container.RegisterSingleton<ISewinQueue, SewinQueue>();
      container.RegisterSingleton<IMahloLogic, MahloLogic>();
      container.RegisterSingleton<IBowAndSkewLogic, BowAndSkewLogic>();
      container.RegisterSingleton<IPatternRepeatLogic, PatternRepeatLogic>();
      container.RegisterInstance<SynchronizationContext>(WindowsFormsSynchronizationContext.Current);

      container.RegisterSingleton<ICarpetProcessor, CarpetProcessor>();
#if DEBUG
      container.Verify();
#endif

      return container;
    }
  }
}
