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
using MahloService.Settings;
using System.Reactive.Linq;
using System.Reactive;

namespace MahloClient
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      try
      {
        using (var container = InitializeContainer())
        {
          Application.Idle += FirstIdle;
          Form form;
          switch (args.FirstOrDefault() ?? string.Empty)
          {
            case "Mahlo":
              form = container.GetInstance<FormMahlo>();
              break;

            case "BowAndSkew":
              form = container.GetInstance<FormBowAndSkew>();
              break;

            default:
              form = container.GetInstance<MainForm>();
              break;
          }

          Application.Run(form);

          async void FirstIdle(object sender, EventArgs e)
          {
            // We need to start the client after the form is run so the event loop is estalished.
            Application.Idle -= FirstIdle;
            var serviceSettings = container.GetInstance<IServiceSettings>();
            var ipcClient = container.GetInstance<IMahloIpcClient>();
            await ipcClient.Start();
            await ipcClient.GetServiceSettings(serviceSettings);
          };
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Program failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private static Container InitializeContainer()
    {
      // This call sets the WindowsFormsSynchronizationContext.Current
      using (new Control()) { };

      Container container = new Container();
      container.RegisterSingleton<IClientSettings, ClientSettings>();

      var registration = Lifestyle.Transient.CreateRegistration<MainForm>(container);
      registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Done by system");

      container.RegisterSingleton<IMahloIpcClient, MahloIpcClient>();
      container.RegisterSingleton<ISewinQueue, SewinQueue>();
      container.RegisterSingleton<IMahloLogic, MahloLogic>();
      container.RegisterSingleton<IBowAndSkewLogic, BowAndSkewLogic>();
      container.RegisterSingleton<IPatternRepeatLogic, PatternRepeatLogic>();
      container.RegisterSingleton<IServiceSettings, ServiceSettings>();
      container.RegisterInstance<SynchronizationContext>(WindowsFormsSynchronizationContext.Current);

      container.RegisterSingleton<ICarpetProcessor, CarpetProcessor>();
#if DEBUG
      container.Verify();
#endif

      return container;
    }
  }
}
