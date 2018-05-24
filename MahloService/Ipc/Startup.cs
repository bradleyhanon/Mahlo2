using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using Serilog;

namespace MahloService.Ipc
{
  class Startup
  {
    public static IDisposable Start(string url)
    {
      IDisposable result;
      try
      {
        result = WebApp.Start<Startup>(url);
        Log.Logger.Debug("SignalR listening on: {url}", url);
      }
      catch(Exception ex)
      {
        Log.Logger.Error(ex, "Unable to start SignalR");
        result = null;
      }

      return result;
    }

    public void Configuration(IAppBuilder app)
    {
      var hubConfiguration = new HubConfiguration();
      hubConfiguration.EnableDetailedErrors = true;

      app.UseCors(CorsOptions.AllowAll);
      app.MapSignalR(hubConfiguration);
    }
  }
}
