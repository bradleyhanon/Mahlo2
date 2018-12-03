using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using Serilog;

namespace MahloService.Ipc
{
  internal class Startup
  {
    public static IDisposable Start(string url)
    {
      IDisposable result;
      try
      {
        result = WebApp.Start<Startup>(url);
        Log.Logger.Information("SignalR listening on: {url}", url);
      }
      catch (Exception ex)
      {
        Log.Logger.Error(ex, "Unable to start SignalR");
        result = null;
      }

      return result;
    }

    public void Configuration(IAppBuilder app)
    {
      var hubConfiguration = new HubConfiguration
      {
        EnableDetailedErrors = true
      };

      app.UseCors(CorsOptions.AllowAll);
      app.MapSignalR(hubConfiguration);
    }
  }
}
