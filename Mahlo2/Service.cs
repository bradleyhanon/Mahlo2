﻿using System;
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
    private SingleThreadSynchronizationContext syncContext;
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
      this.syncContext = new SingleThreadSynchronizationContext();
      this.serviceThread = new Thread(this.ThreadProc);
      this.serviceThread.Start(args);
    }

    protected override void OnStop()
    {
      if (!this.isStopping)
      {
        this.isStopping = true;
        this.syncContext.Complete();
        this.serviceThread.Join();
      }
    }

    void ThreadProc(object args)
    {
      try
      {
        SynchronizationContext.SetSynchronizationContext(syncContext);
        using (var container = Program.InitializeContainer(syncContext))
        {
          this.log = container.GetInstance<ILogger>();
          var mapper = container.GetInstance<ICarpetProcessor>();
          mapper.Start();

          log.Information("Service started");
          syncContext.RunOnCurrentThread();
          log.Information("Service stopped");
        }
      }
      catch (Exception ex)
      {
        log.Error("Service thread error.", ex);
      }
    }
  }
}
