using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mahlo.AppSettings;
using Mahlo.Logic;
using Mahlo.Opc;
using Mahlo.Repository;
using Mahlo.Views;
using OpcLabs.EasyOpc;
using OpcLabs.EasyOpc.DataAccess;
using SimpleInjector;
using SimpleInjector.Diagnostics;

namespace Mahlo
{
  class Program
  {
    //private static Container container;

    [STAThread]
    private static void Main(string[] args)
    {
      int[] x = { 1, 2, 3, 4 };
      
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
      try
      {
        string instanceName = @"Local\{7A97254E-AFC8-4C0C-A6DB-C6DA0BFB463F}";
        using (new SingleInstance(instanceName))
        {
          Application.EnableVisualStyles();
          Application.SetCompatibleTextRenderingDefault(false);
          using (var container = InitializeContainer())
          {
            if (args.Contains("--migrate"))
            {
              var runner = container.GetInstance<Mahlo.DbMigrations.Runner>();
              runner.MigrateToLatest();
              Environment.Exit(0);
            }

            container.GetInstance<CarpetProcessor>();
            Application.Run(container.GetInstance<MainForm>());
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

    private static Container InitializeContainer()
    {
      var container = new Container();

      var registration = Lifestyle.Transient.CreateRegistration<MainForm>(container);
      registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Done by system");

      // This call sets the WindowsFormsSynchronizationContext.Current
      using (new Control()) { } ;

      //container.RegisterSingleton<MainForm>(new MainForm());
      container.RegisterSingleton<SynchronizationContext>(WindowsFormsSynchronizationContext.Current);
      container.RegisterSingleton<ISewinQueue, SewinQueue>();
      container.RegisterSingleton<IMahloLogic, MahloLogic>();
      container.RegisterSingleton<IBowAndSkewLogic, BowAndSkewLogic>();
      container.RegisterSingleton<IPatternRepeatLogic, PatternRepeatLogic>();
      container.RegisterSingleton<ICutRollLogic, CutRollLogic>();
      container.RegisterSingleton<IBowAndSkewSrc>(() => container.GetInstance<MahloOpcClient>());
      container.RegisterSingleton<IPatternRepeatSrc>(() => container.GetInstance<MahloOpcClient>());
      container.RegisterSingleton<IMeterSrc>(() => container.GetInstance<MahloOpcClient>());
      container.RegisterSingleton<EasyDAClient>(() => new EasyDAClient());

      container.RegisterSingleton<IDbConnectionFactoryFactory, DbConnectionFactory.Factory>();
      container.RegisterSingleton<IDbMfg, DbMfg>();
      container.RegisterSingleton<IDbLocal, DbLocal>();
      container.RegisterSingleton<IAppInfoBAS, AppInfoBAS>();
      container.RegisterSingleton<IOpcSettings, OpcSettings>();
      container.RegisterSingleton<IMahloOpcSettings, MahloOpcSettings>();
      container.RegisterSingleton<IPlcSettings, PlcSettings>();

      container.Verify();
      return container;
    }
  }
}
