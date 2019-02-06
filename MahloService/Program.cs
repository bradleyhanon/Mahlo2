using System;
using System.Configuration.Install;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using MahloService.Ipc;
using MahloService.Logging;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Settings;
using MahloService.Simulation;
using MahloService.Utilities;
using OpcLabs.EasyOpc.DataAccess;
using Serilog;
using SimpleInjector;

namespace MahloService
{
  internal class Program
  {
    public const string StrMahloMapper = "MahloMapper";
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
            .ReadFrom.AppSettings()
            .CreateLogger();

          if (Environment.UserInteractive)
          {
            if (args.Contains("--migrate"))
            {
              var runner = new MahloService.DbMigrations.Runner(new DbLocal(new DbConnectionFactory.Factory()));
              runner.MigrateToLatest();
              Environment.Exit(0);
            }

            if (args.Contains("--install"))
            {
              var runner = new MahloService.DbMigrations.Runner(new DbLocal(new DbConnectionFactory.Factory()));
              runner.MigrateToLatest();
              ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
              new ServiceController(StrMahloMapper).Start();
              Environment.Exit(0);
            }

            if (args.Contains("--uninstall"))
            {
              ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
              Environment.Exit(0);
            }

            bool shouldSimulate = args.Contains("--simulate");
            if (shouldSimulate)
            {
              Application.EnableVisualStyles();
              Application.SetCompatibleTextRenderingDefault(false);
              using (new Control()) { }
              using (var container = InitializeContainer(WindowsFormsSynchronizationContext.Current, shouldSimulate))
              {
                ICarpetProcessor carpetProcessor = container.GetInstance<ICarpetProcessor>();
                carpetProcessor.Start();
                Application.Run(container.GetInstance<FormSim>());
                container.GetInstance<IProgramState>().Save();
                carpetProcessor.Stop();
              }
            }
            else
            {
              ApplicationContext appContext = new ApplicationContext();
              WindowsFormsSynchronizationContext.AutoInstall = false;

              WindowsFormsSynchronizationContext syncContext = new WindowsFormsSynchronizationContext();
              SynchronizationContext.SetSynchronizationContext(syncContext);

              using (var consoleCtrl = new ConsoleCtrl())
              using (var container = InitializeContainer(syncContext, shouldSimulate))
              {
                consoleCtrl.ControlEvent += (sender, e) =>
                {
                  Application.Exit();
                  e.Result = true;
                };

                Log.Logger.Information("Application started");
                ICarpetProcessor carpetProcessor = container.GetInstance<ICarpetProcessor>();
                carpetProcessor.Start();
                Application.Run(appContext);
                container.GetInstance<IProgramState>().Save();
                carpetProcessor.Stop();
                Log.Logger.Information("Application stopped");
              }
            }
          }
          else
          {
            service = new Service();
            ServiceBase.Run(service);
          }
        }
      }
      catch (SingleInstanceException)
      {
        Environment.Exit(1);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), Application.ProductName);
        Environment.Exit(1);
      }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      string message = "Unhandled Exception: \n" + e.ExceptionObject.ToString();
      Log.Logger.Error(message);
      if (Environment.UserInteractive)
      {
        MessageBox.Show(message, Application.ProductName);
      }

      Environment.Exit(1);
    }

    public static Container InitializeContainer(SynchronizationContext syncContext, bool shouldSimulate)
    {
      var container = new Container();
      Program.Container = container;

      container.Options.DependencyInjectionBehavior =
        new SerilogContextualLoggerInjectionBehavior(container.Options);

      container.RegisterInstance(Log.Logger);
      container.RegisterSingleton<IProgramState, ProgramState>();
      container.RegisterSingleton<IProgramStateProvider, DbLocal>();

      container.RegisterSingleton<ICriticalStops<MahloModel>, CriticalStops<MahloModel>>();
      container.RegisterSingleton<ICriticalStops<BowAndSkewModel>, CriticalStops<BowAndSkewModel>>();
      container.RegisterSingleton<ICriticalStops<PatternRepeatModel>, CriticalStops<PatternRepeatModel>>();

      container.RegisterSingleton<IUserAttentions<MahloModel>, UserAttentions<MahloModel>>();
      container.RegisterSingleton<IUserAttentions<BowAndSkewModel>, UserAttentions<BowAndSkewModel>>();
      container.RegisterSingleton<IUserAttentions<PatternRepeatModel>, UserAttentions<PatternRepeatModel>>();

      container.RegisterSingleton<IMahloLogic, MahloLogic>();
      container.RegisterSingleton<IBowAndSkewLogic, BowAndSkewLogic>();
      container.RegisterSingleton<IPatternRepeatLogic, PatternRepeatLogic>();
      container.RegisterSingleton<ISapRollAssigner, SapRollAssigner>();

      if (shouldSimulate)
      {
        container.RegisterSingleton<IOpcServerController, OpcServerControllerSim>();
        container.RegisterSingleton<IMahloSrc, OpcSrcSim<MahloModel>>();
        container.RegisterSingleton<IBowAndSkewSrc, OpcSrcSim<BowAndSkewModel>>();
        container.RegisterSingleton<IPatternRepeatSrc, OpcSrcSim<PatternRepeatModel>>();
        container.RegisterSingleton<IDbMfgSim, DbMfgSim>();
        container.RegisterSingleton<IDbMfg>(() => container.GetInstance<IDbMfgSim>());
      }
      else
      {
        container.RegisterSingleton<IOpcServerController, OpcServerController>();
        container.RegisterSingleton<IMahloSrc, OpcClient<MahloModel>>();
        container.RegisterSingleton<IBowAndSkewSrc, OpcClient<BowAndSkewModel>>();
        container.RegisterSingleton<IPatternRepeatSrc, OpcClient<PatternRepeatModel>>();
        container.RegisterSingleton<IEasyDAClient>(() => new EasyDAClient());
        container.RegisterSingleton<IDbMfg, DbMfg>();
      }

      //container.RegisterSingleton<IMeterSrc<MahloRoll>, OpcClient<MahloRoll>>();
      //container.RegisterSingleton<IMeterSrc<BowAndSkewRoll>, OpcClient<BowAndSkewRoll>>();
      //container.RegisterSingleton<IMeterSrc<PatternRepeatRoll>, OpcClient<PatternRepeatRoll>>();

      container.RegisterInstance<IScheduler>(new SynchronizationContextScheduler(syncContext));
      container.RegisterInstance<SynchronizationContext>(syncContext);
      container.RegisterSingleton<IConcurrencyInfo, ConcurrencyInfo>();
      container.RegisterSingleton<ISewinQueue, SewinQueue>();
      container.RegisterSingleton<CutRollList>();
      container.RegisterSingleton<ICutRollLogic, CutRollLogic>();
      container.RegisterSingleton<InspectionAreaList>();

      container.RegisterSingleton<IDbConnectionFactoryFactory, DbConnectionFactory.Factory>();
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
