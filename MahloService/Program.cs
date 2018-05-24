using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahloService.Settings;
using MahloService.Ipc;
using MahloService.Logging;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Utilities;
using OpcLabs.EasyOpc;
using OpcLabs.EasyOpc.DataAccess;
using Serilog;
using SimpleInjector;
using SimpleInjector.Diagnostics;

namespace MahloService
{
  class Program
  {
    private static Service service;

    public static Container Container { get; private set; }

    [STAThread]
    private static void Main(string[] args)
    {      
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
      try
      {
        string instanceName = @"Local\{7A97254E-AFC8-4C0C-A6DB-C6DA0BFB463F}";
        using (new SingleInstance(instanceName))
        {
          Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.EventLog("MahloMapper", manageEventSource: true, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
            //.ReadFrom.AppSettings()
            .CreateLogger();

          if (Environment.UserInteractive)
          {
            if (args.Contains("--migrate"))
            {
              //var runner = container.GetInstance<Mahlo.DbMigrations.Runner>();
              var runner = new MahloService.DbMigrations.Runner(new DbLocal(new DbConnectionFactory.Factory()));
              runner.MigrateToLatest();
              Environment.Exit(0);
            }

            if (args.Contains("--install"))
            {
              ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
              Environment.Exit(0);
            }

            if (args.Contains("--uninstall"))
            {
              ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
              Environment.Exit(0);
            }

            SingleThreadSynchronizationContext syncContext = new SingleThreadSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncContext);
            using (var consoleCtrl = new ConsoleCtrl())
            using (var container = InitializeContainer(syncContext))
            {
              consoleCtrl.ControlEvent += (sender, e) =>
              {
                syncContext.Complete();
                e.Result = true;
              };

              Log.Logger.Information("Applicaiton started");
              container.GetInstance<ICarpetProcessor>().Start();
              syncContext.RunOnCurrentThread();
              Log.Logger.Information("Application stopped");
            }
          }
          else
          {
            service = new Service();
            ServiceBase.Run(service);
          }
        }
      }
      catch (SingleInstance.Exception)
      {
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), Application.ProductName);
      }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      MessageBox.Show("Unhandled Exception: \n" + e.ExceptionObject.ToString(), Application.ProductName);
      Environment.Exit(1);
    }

    public static Container InitializeContainer(SynchronizationContext syncContext)
    {
      var container = new Container();
      Program.Container = container;

      container.Options.DependencyInjectionBehavior =
        new SerilogContextualLoggerInjectionBehavior(container.Options);

      // Register IProgramState first so it will be the last disposed
      container.RegisterSingleton<IProgramState, ProgramState>();
      container.RegisterSingleton<IProgramStateProvider, DbLocal>();

      container.RegisterSingleton<ICriticalStops<MahloRoll>, CriticalStops<MahloRoll>>();
      container.RegisterSingleton<ICriticalStops<BowAndSkewRoll>, CriticalStops<BowAndSkewRoll>>();
      container.RegisterSingleton<ICriticalStops<PatternRepeatRoll>, CriticalStops<PatternRepeatRoll>>();

      container.RegisterSingleton<IUserAttentions<MahloRoll>, UserAttentions<MahloRoll>>();
      container.RegisterSingleton<IUserAttentions<BowAndSkewRoll>, UserAttentions<BowAndSkewRoll>>();
      container.RegisterSingleton<IUserAttentions<PatternRepeatRoll>, UserAttentions<PatternRepeatRoll>>();

      container.RegisterSingleton<IMahloLogic, MahloLogic>();
      container.RegisterSingleton<IBowAndSkewLogic, BowAndSkewLogic>();
      container.RegisterSingleton<IPatternRepeatLogic, PatternRepeatLogic>();

      container.RegisterSingleton<IMahloSrc<MahloRoll>, MahloOpcClient<MahloRoll>>();
      container.RegisterSingleton<IBowAndSkewSrc<BowAndSkewRoll>, MahloOpcClient<BowAndSkewRoll>>();
      container.RegisterSingleton<IPatternRepeatSrc<PatternRepeatRoll>, MahloOpcClient<PatternRepeatRoll>>();

      container.RegisterSingleton<IMeterSrc<MahloRoll>, MahloOpcClient<MahloRoll>>();
      container.RegisterSingleton<IMeterSrc<BowAndSkewRoll>, MahloOpcClient<BowAndSkewRoll>>();
      container.RegisterSingleton<IMeterSrc<PatternRepeatRoll>, MahloOpcClient<PatternRepeatRoll>>();

      container.RegisterSingleton<ISchedulerProvider, SchedulerProvider>();
      container.RegisterInstance<SynchronizationContext>(syncContext);
      container.RegisterSingleton<IConcurrencyInfo, ConcurrencyInfo>();
      container.RegisterSingleton<ISewinQueue, SewinQueue>();
      container.RegisterSingleton<ICutRollLogic, CutRollLogic>();
      container.RegisterSingleton<EasyDAClient>(() => new EasyDAClient());

      container.RegisterSingleton<IDbConnectionFactoryFactory, DbConnectionFactory.Factory>();
      container.RegisterSingleton<IDbMfg, DbMfg>();
      container.RegisterSingleton<IDbLocal, DbLocal>();
      container.RegisterSingleton<IServiceSettings, ServiceSettings>();
      container.RegisterSingleton<IOpcSettings, OpcSettings>();
      container.RegisterSingleton<ICarpetProcessor, CarpetProcessor>();
      container.RegisterSingleton<IMahloServer, MahloServer>();

      container.Verify();
      return container;
    }
  }
}
