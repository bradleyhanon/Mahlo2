using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapperClient.AppSettings;
using MapperClient.Views;
using SimpleInjector;

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
      using (var container = InitializeContainer())
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(container.GetInstance<MainForm>());
      }
    }

    private static Container InitializeContainer()
    {
      Container container = new Container();
      container.RegisterSingleton<IAppInfo, AppInfo>();

#if DEBUG
      container.Verify();
#endif

      return container;
    }
  }
}
