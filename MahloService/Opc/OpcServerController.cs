using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MahloService.Opc;
using MahloService.Settings;
using Serilog;

namespace MahloService.Opc
{
  internal class OpcServerController : IOpcServerController
  {
    const string OpcServerWindowCaption = "mahlo 10A OPC-Server";

    private const int GW_HWNDNEXT = 2;
    private const int WM_GETTEXT = 13;
    private const int WM_GETTEXTLENGTH = 14;
    private const int WM_CLOSE = 16;

    private readonly IOpcSettings opcSettings;
    private readonly ILogger logger;
    private readonly string processName;

    private Process process;

    public OpcServerController(IOpcSettings opcSettings, ILogger logger)
    {
      this.opcSettings = opcSettings;
      this.logger = logger;
      this.processName = Path.GetFileNameWithoutExtension(opcSettings.OpcServerPath);
    }

    public void Start()
    {
      // If we're just starting, we don't have the OpcServer process information 
      // so get the information if it is running
      if (this.process == null)
      {
        // 10AOpcServer starts another instance of itself with a command line argument when a client connects.
        // We want to get the main OPC server process, (the one without a command line argument)
        var processes = Process.GetProcessesByName(this.processName).Where(p => string.IsNullOrWhiteSpace(p.StartInfo.Arguments)).ToArray();
        if (processes.Length > 1)
        {
          this.logger.Error($"{processes.Length} OPC servers are running!");
        }

        this.process = processes.FirstOrDefault();
      }

      if (this.process != null)
      {
        // The process is already running
        this.logger.Information("The OPC server is already running.");
        return;
      }

      try
      {
        // We need to start it.
        ProcessStartInfo startInfo = new ProcessStartInfo(this.opcSettings.OpcServerPath)
        {
          WindowStyle = ProcessWindowStyle.Minimized
        };

        this.process = Process.Start(startInfo);
        this.process.WaitForInputIdle();
        this.logger.Information("OPC server started.");
      }
      catch (Exception ex)
      {
        this.logger.Error($"Unable to start OPC server: {this.opcSettings.OpcServerPath}\n{ex.Message}");
      }
    }
  }
}
