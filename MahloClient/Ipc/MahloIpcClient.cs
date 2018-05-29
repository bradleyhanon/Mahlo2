using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahloService.Ipc;
using MahloService.Logic;
using MahloService.Models;
using MahloClient.AppSettings;
using MahloClient.Logic;
using MahloClient.Views;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MahloService.Settings;

namespace MahloClient.Ipc
{
  sealed class MahloIpcClient : IMahloIpcClient, IDisposable
  {
    public const string MoveToNextRollCommand = "MoveToNextRoll";
    public const string MoveToPriorRollCommand = "MoveToPriorRoll";
    public const string WaitForSeamCommand = "WaitForSeam";
    public const string DisableSystemCommand = "DisableSystem";

    private bool IsDisposed;
    private HubConnection hubConnection;
    private IHubProxy hubProxy;
    private IClientSettings appInfo;
    private ISewinQueue sewinQueue;
    private SynchronizationContext context;

    private bool isStarting;
    private string connectionError;
    private TaskCompletionSource<object> connectionTcs = new TaskCompletionSource<object>();
    private string ipcStatusMessage;

    public MahloIpcClient(
      ISewinQueue sewinQueue,
      IClientSettings appInfo,
      SynchronizationContext context)
    {
      this.appInfo = appInfo;
      this.sewinQueue = sewinQueue;
      this.context = context;
    }

    public string IpcStatusMessage
    {
      get => this.ipcStatusMessage;
      set
      {
        if (this.ipcStatusMessage != value)
        {
          this.ipcStatusMessage = value;
          this.context.Post(_ => this.IpcStatusMessageChanged?.Invoke(value), null);
        }
      }
    }

    public event Action<(string name, JObject jObject)> MeterLogicUpdated;
    public event Action<string> IpcStatusMessageChanged;

    public ConnectionState State => this.hubConnection?.State ?? ConnectionState.Disconnected;

    public void Dispose()
    {
      this.IsDisposed = true;
      this.hubConnection.Dispose();
    }

    public async Task Start()
    {
      if (!isStarting)
      {
        Console.WriteLine("Connection starting");
        this.isStarting = true;

        this.hubConnection = new HubConnection(appInfo.ServiceUrl);
        this.hubConnection.StateChanged += HubConnection_StateChanged;
        this.hubConnection.Received += msg => Console.WriteLine(msg);
        //this.hubConnection.TraceLevel = TraceLevels.All;
        //this.hubConnection.TraceWriter = Console.Out;
        this.hubProxy = hubConnection.CreateHubProxy("MahloHub");
        this.hubProxy.On("UpdateSewinQueue", arg =>
        {
          this.context.Post(_ =>
            this.sewinQueue.UpdateSewinQueue(arg.ToObject<CarpetRoll[]>()), null);
        });

        this.hubProxy.On<string, JObject>("UpdateMeterLogic", (name, arg) => this.context.Post(_ =>
        {
          this.MeterLogicUpdated?.Invoke((name, arg));
        }, null));


        do
        {
          try
          {
            await hubConnection.Start();
            this.IpcStatusMessage = this.hubConnection.State.ToString();
          }
          catch (HttpClientException ex) when (ex.Response != null)
          {
            this.IpcStatusMessage = ex.Response.ReasonPhrase;
            await Task.Delay(3000);
          }
          catch (Exception ex)
          {
            var dr = MessageBox.Show($"Unable to connect to Mahlo service\n\n{ex.Message}", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            if (dr == DialogResult.Cancel)
            {
              Environment.Exit(1);
            }
          }
        } while (this.hubConnection.State != ConnectionState.Connected);

        this.isStarting = false;
      }
    }

    private void HubConnection_Received(string obj)
    {
      throw new NotImplementedException();
    }

    public Task<(string message, string caption)> BasSetRecipe(string rollNo, string styleCode, string recipeName, RecipeApplyToEnum applyTo)
    {
      bool isManualMode = recipeName == FormSetRecipe.ManualModeRecipeName;
      return this.Call<(string, string)>(nameof(BasSetRecipe), rollNo, styleCode, recipeName, isManualMode, applyTo);
    }

    public Task<IEnumerable<CoaterScheduleRoll>> GetCoaterSchedule(int minSequence, int maxSequence)
    {
      return this.Call<IEnumerable<CoaterScheduleRoll>>(nameof(GetCoaterSchedule), minSequence, maxSequence);
    }

    public async Task GetServiceSettings(IServiceSettings serviceSettings)
    {
      JObject obj = await this.Call<JObject>(nameof(GetServiceSettings));
      obj.Populate(serviceSettings);
    }

    private async void HubConnection_StateChanged(StateChange obj)
    {
      Console.WriteLine($"OldState={obj.OldState}, NewState={obj.NewState}");
      this.IpcStatusMessage = obj.NewState.ToString();
      
      if (obj.NewState == ConnectionState.Connected)
      {
        this.connectionTcs.SetResult(null);
        this.context.Post(_ => Call("RefreshAll"), null);
      }
      else if (obj.OldState == ConnectionState.Connected)
      {
        this.connectionTcs = new TaskCompletionSource<object>();
      }

      if (!this.isStarting && obj.NewState == ConnectionState.Disconnected && !this.IsDisposed)
      {
        await Task.Delay(5000);
        await this.Start();
      }
    }

    private bool RetryCheck(string method, Exception ex)
    {
      var dr = MessageBox.Show("Try again?", "Communication Failure", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
      return dr == DialogResult.Retry;
    }

    private void SetConnectionError(Exception ex)
    {
      StringBuilder builder = new StringBuilder();
      for (; ; )
      {
        builder.Append(ex.Message);
        ex = ex.InnerException;
        if (ex == null)
        {
          break;
        }

        builder.AppendLine();
      }

      connectionError = builder.ToString();
    }

    /// <summary>
    /// Executes a method on the server side hub asynchronously.
    /// </summary>
    /// <param name="method">The name of the method.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>A task that represents when invocation returned.</returns>
    public Task Call(string method, params object[] args)
    {
      return this.Call<object>(method, args);
    }

    public async Task<T> Call<T>(string method, params object[] args)
    {
      //Console.Clear();
      StringBuilder builder = new StringBuilder();
      builder.Append(method);
      builder.Append('(');
      var separator = "";
      foreach (var arg in args)
      {
        builder.Append(separator);
        builder.Append(arg.ToString());
        separator = ",";
      }

      builder.Append(");");

      Console.WriteLine(builder.ToString());

      T result = default;
      for (; ; )
      {
        try
        {
          await this.connectionTcs.Task;
          result = await hubProxy.Invoke<T>(method, args);
          break;
        }
        catch (InvalidOperationException)
        {

        }
        catch (Exception ex) when (this.hubConnection.State != ConnectionState.Connected)
        {
          SetConnectionError(ex);
          if (!RetryCheck(method, ex))
          {
            break;
          }
        }
      }

      return result;
    }
  }
}
