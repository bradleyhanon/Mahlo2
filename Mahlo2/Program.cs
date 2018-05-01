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
using Mahlo.AppSettings;
using Mahlo.Ipc;
using Mahlo.Logging;
using Mahlo.Logic;
using Mahlo.Models;
using Mahlo.Opc;
using Mahlo.Repository;
using Mahlo.Utilities;
using Mahlo.Views;
using OpcLabs.EasyOpc;
using OpcLabs.EasyOpc.DataAccess;
using Serilog;
using SimpleInjector;
using SimpleInjector.Diagnostics;

namespace Mahlo
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
              var runner = new Mahlo.DbMigrations.Runner(new DbLocal(new DbConnectionFactory.Factory()));
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var container = InitializeContainer())
            {
              Log.Logger.Information("Applicaiton started");
              Application.Run(container.GetInstance<MainForm>());
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

    public static Container InitializeContainer()
    {
      var container = new Container();
      Program.Container = container;

      container.Options.DependencyInjectionBehavior =
        new SerilogContextualLoggerInjectionBehavior(container.Options);

      // Register IProgramState first so it will be the last disposed
      container.RegisterSingleton<IProgramState, ProgramState>();
      container.RegisterSingleton<IProgramStateProvider, DbLocal>();

      var registration = Lifestyle.Transient.CreateRegistration<MainForm>(container);
      registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Done by system");

      // This call sets the WindowsFormsSynchronizationContext.Current
      using (new Control()) { } ;

      container.RegisterSingleton<ICriticalStops<MahloRoll>, CriticalStops<MahloRoll>>();
      container.RegisterSingleton<ICriticalStops<BowAndSkewRoll>, CriticalStops<BowAndSkewRoll>>();
      container.RegisterSingleton<ICriticalStops<PatternRepeatRoll>, CriticalStops<PatternRepeatRoll>>();

      container.RegisterSingleton<IUserAttentions<MahloRoll>, UserAttentions<MahloRoll>>();
      container.RegisterSingleton<IUserAttentions<BowAndSkewRoll>, UserAttentions<BowAndSkewRoll>>();
      container.RegisterSingleton<IUserAttentions<PatternRepeatRoll>, UserAttentions<PatternRepeatRoll>>();

      container.RegisterSingleton<IMahloLogic, MahloLogic>();
      container.RegisterSingleton<IBowAndSkewLogic, BowAndSkewLogic>();
      container.RegisterSingleton<IPatternRepeatLogic, PatternRepeatLogic>();

      //container.RegisterSingleton<IMeterLogic<MahloRoll>, MeterLogic<MahloRoll>>();
      //container.RegisterSingleton<IMeterLogic<BowAndSkewRoll>, MeterLogic<BowAndSkewRoll>>();
      //container.RegisterSingleton<IMeterLogic<PatternRepeatRoll>, MeterLogic<PatternRepeatRoll>>();

      //container.RegisterSingleton<IRollLengthMonitor<MahloRoll>, RollLengthMonitor<MahloRoll>>();
      //container.RegisterSingleton<IRollLengthMonitor<BowAndSkewRoll>, RollLengthMonitor<BowAndSkewRoll>>();
      //container.RegisterSingleton<IRollLengthMonitor<PatternRepeatRoll>, RollLengthMonitor<PatternRepeatRoll>>();

      container.RegisterSingleton<IMahloSrc<MahloRoll>, MahloOpcClient<MahloRoll>>();
      container.RegisterSingleton<IBowAndSkewSrc<BowAndSkewRoll>, MahloOpcClient<BowAndSkewRoll>>();
      container.RegisterSingleton<IPatternRepeatSrc<PatternRepeatRoll>, MahloOpcClient<PatternRepeatRoll>>();

      container.RegisterSingleton<IMeterSrc<MahloRoll>, MahloOpcClient<MahloRoll>>();
      container.RegisterSingleton<IMeterSrc<BowAndSkewRoll>, MahloOpcClient<BowAndSkewRoll>>();
      container.RegisterSingleton<IMeterSrc<PatternRepeatRoll>, MahloOpcClient<PatternRepeatRoll>>();

      container.RegisterSingleton<ISchedulerProvider, SchedulerProvider>();
      container.RegisterInstance<SynchronizationContext>(WindowsFormsSynchronizationContext.Current);
      container.RegisterSingleton<IConcurrencyInfo, ConcurrencyInfo>();
      container.RegisterSingleton<ISewinQueue, SewinQueue>();
      container.RegisterSingleton<ICutRollLogic, CutRollLogic>();
      container.RegisterSingleton<EasyDAClient>(() => new EasyDAClient());

      container.RegisterSingleton<IDbConnectionFactoryFactory, DbConnectionFactory.Factory>();
      container.RegisterSingleton<IDbMfg, DbMfg>();
      container.RegisterSingleton<IDbLocal, DbLocal>();
      container.RegisterSingleton<IAppInfoBAS, AppInfoBAS>();
      //container.RegisterSingleton<IOpcSettings, OpcSettings>();
      container.RegisterSingleton<IMahloOpcSettings, MahloOpcSettings>();
      //container.RegisterSingleton<IPlcSettings, PlcSettings>();
      container.RegisterSingleton<CarpetProcessor>();
      container.RegisterSingleton<IMahloServer, MahloServer>();
      //container.Register<MahloHub>(() => new MahloHub());

      container.Verify();
      return container;
    }
  }
}
