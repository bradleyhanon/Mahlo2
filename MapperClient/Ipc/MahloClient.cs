using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mahlo.Logic;
using Mahlo.Models;
using MapperClient.AppSettings;
using MapperClient.Logic;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MapperClient.Ipc
{
  class MahloClient
  {
    private HubConnection hubConnection;
    private IHubProxy proxy;
    private IAppInfo appInfo;
    private ISewinQueue sewinQueue;
    private SynchronizationContext context;

    private bool isStarting;

    public MahloClient(ISewinQueue sewinQueue, IAppInfo appInfo, SynchronizationContext context)
    {
      this.appInfo = appInfo;
      this.sewinQueue = sewinQueue;
      this.context = context;
    }

    public async void Start()
    {
      if (!isStarting)
      {
        this.isStarting = true;

        this.hubConnection = new HubConnection(appInfo.ServerUrl);
        this.proxy = hubConnection.CreateHubProxy("MahloHub");
        this.proxy.On("UpdateSewinQueue", arg =>
        {
          this.context.Post(_ =>
            this.sewinQueue.UpdateSewinQueue(arg.ToObject<CarpetRoll[]>()), null);
        });
        //{
        //  JArray array = arg;
        //  IEnumerable<CarpetRoll> rolls = array.ToObject<CarpetRoll[]>();
        //  this.sewinQueue.UpdateSewinQueue(rolls);
        //});

        do
        {
          try
          {
            await hubConnection.Start();
          }
          catch (Exception ex)
          {
            var dr = MessageBox.Show("Unable to connect to MahloMapper service", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            if (dr == DialogResult.Cancel)
            {
              Environment.Exit(1);
            }
          }
        } while (this.hubConnection.State != ConnectionState.Connected);

        this.isStarting = false;
      }
    }
  }
}
