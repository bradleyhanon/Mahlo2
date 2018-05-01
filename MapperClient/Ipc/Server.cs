using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using MapperClient.AppSettings;
using MapperClient.Logic;
using Microsoft.AspNet.SignalR.Client;

namespace MapperClient.Ipc
{
  class Server
  {
    HubConnection hubConnection;
    IHubProxy proxy;
    IAppInfo appInfo;
    ISewinQueue sewinQueue;

    public Server(ISewinQueue sewinQueue, IAppInfo appInfo)
    {
      this.appInfo = appInfo;
      this.sewinQueue = sewinQueue;
    }

    public void Start()
    {
      this.hubConnection = new HubConnection(appInfo.ServerUrl);
      this.proxy = hubConnection.CreateHubProxy("MahloHub");
      this.proxy.On("SewinQueueChanged", rolls => this.sewinQueue.UpdateSewinQueue(rolls));

      hubConnection.Start();
    }
  }
}
