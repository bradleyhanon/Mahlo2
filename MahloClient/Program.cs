﻿using System;
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
          Application.Run(container.GetInstance<FormBowAndSkew>());

          async void FirstIdle(object sender, EventArgs e)
          {
            // We need to start the client after the form is run so the event loop is estalished.
            var serviceSettings = container.GetInstance<IServiceSettings>();
            MahloIpcClient mahloIpcClient = container.GetInstance<MahloIpcClient>();
            await mahloIpcClient.Start();
            await mahloIpcClient.GetServiceSettings(serviceSettings);
            Application.Idle -= FirstIdle;
          };
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Program failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private static void Application_Idle(object sender, EventArgs e)
    {
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
