using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mahlo.Logic;
using Mahlo.Models;
using MapperClient.AppSettings;
using MapperClient.Ipc;
using MapperClient.Logic;
using MapperClient.Views;
using SimpleInjector;
using SimpleInjector.Diagnostics;

namespace MapperClient
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
        var notUsed = container.GetInstance<MahloClient>().Start();
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

      container.RegisterSingleton<IMahloClient, MahloClient>();
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
