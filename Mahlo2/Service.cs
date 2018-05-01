using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mahlo.Logic;
using Serilog;

namespace Mahlo
{
  partial class Service : ServiceBase
  {
    private ManualResetEvent terminateEvent = new ManualResetEvent(false);
    private Thread serviceThread;
    private bool isStopping;
    private ILogger log;

    public Service()
    {
      InitializeComponent();
    }

    protected override void OnStart(string[] args)
    {
      this.isStopping = false;
      this.terminateEvent.Reset();
      this.serviceThread = new Thread(this.ThreadProc);
      this.serviceThread.Start(args);
    }

    protected override void OnStop()
    {
      if (!this.isStopping)
      {
        this.isStopping = true;
        this.terminateEvent.Set();
        this.serviceThread.Join();
      }
    }

    void ThreadProc(object args)
    {
      try
      {
        using (var container = Program.InitializeContainer())
        {
          this.log = container.GetInstance<ILogger>();
          var mapper = container.GetInstance<CarpetProcessor>();
          mapper.Start();
        }

        log.Information("Service started");
        this.terminateEvent.WaitOne();
        log.Information("Service stopped");
      }
      catch (Exception ex)
      {
        log.Error("Service thread error.", ex);
      }
    }
  }
}
