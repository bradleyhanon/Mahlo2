using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahloClient.AppSettings;
using MahloClient.Logic;
using MahloClient.Views;
using MahloService.Ipc;
using MahloService.Models;
using MahloService.Settings;
using MahloService.Utilities;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json.Linq;

namespace MahloClient.Ipc
{
  internal sealed class MahloIpcClient : IMahloIpcClient, IDisposable
  {
    public const string MoveToNextRollCommand = "MoveToNextRoll";
    public const string MoveToPriorRollCommand = "MoveToPriorRoll";
    public const string WaitForSeamCommand = "WaitForSeam";
    public const string DisableSystemCommand = "DisableSystem";

    private bool IsDisposed;
    private HubConnection hubConnection;
    private IHubProxy hubProxy;
    private readonly IClientSettings appInfo;
    private readonly ISewinQueue sewinQueue;
    private readonly ICutRollList cutRollList;
    private readonly SynchronizationContext context;

    private bool isStarting;
    private string connectionError;
    private TaskCompletionSource<object> connectionTcs = new TaskCompletionSource<object>();
    private string ipcStatusMessage;

    public MahloIpcClient(
      ISewinQueue sewinQueue,
      ICutRollList cutRollList,
      IClientSettings appInfo,
      SynchronizationContext context)
    {
      this.appInfo = appInfo;
      this.sewinQueue = sewinQueue;
      this.cutRollList = cutRollList;
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
          TaskUtilities.RunOnMainThreadAsync(() => this.IpcStatusMessageChanged?.Invoke(value)).NoWait();
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

    public async Task StartAsync()
    {
      if (!this.isStarting)
      {
        this.isStarting = true;

        this.hubConnection = new HubConnection(this.appInfo.ServiceUrl);
        this.hubConnection.StateChanged += this.HubConnection_StateChanged;
        this.hubConnection.Received += msg => Console.WriteLine(msg);
        //this.hubConnection.TraceLevel = TraceLevels.All;
        //this.hubConnection.TraceWriter = Console.Out;
        this.hubProxy = this.hubConnection.CreateHubProxy("MahloHub");
        this.hubProxy.On<JArray>("UpdateSewinQueue", arg =>
          TaskUtilities.RunOnMainThreadAsync(() => this.sewinQueue.UpdateSewinQueue(arg)).NoWait());

        this.hubProxy.On<string, JObject>("UpdateMeterLogic", (name, arg) =>
          TaskUtilities.RunOnMainThreadAsync(() => this.MeterLogicUpdated?.Invoke((name, arg))).NoWait());

        this.hubProxy.On<JArray>("UpdateCutRollList", array =>
          TaskUtilities.RunOnMainThreadAsync(() => this.cutRollList.Update(array)).NoWait());

        while (true)
        {
          try
          {
            await this.hubConnection.Start();
            this.IpcStatusMessage = this.hubConnection.State.ToString();
            break;
          }
          catch (HttpClientException ex) when (ex.Response != null)
          {
            this.IpcStatusMessage = ex.Response.ReasonPhrase;
            await Task.Delay(3000);
          }
          catch (Exception)
          {
            await Task.Delay(3000);
          }
        }

        this.isStarting = false;
      }
    }

    private void HubConnection_Received(string obj)
    {
      throw new NotImplementedException();
    }

    public Task<(string message, string caption)> BasSetRecipeAsync(string rollNo, string styleCode, string recipeName, RecipeApplyToEnum applyTo)
    {
      bool isManualMode = recipeName == FormSetRecipe.ManualModeRecipeName;
      return this.CallAsync<(string, string)>("BasSetRecipe", rollNo, styleCode, recipeName, isManualMode, applyTo);
    }

    public Task<IEnumerable<CoaterScheduleRoll>> GetCoaterScheduleAsync(int minSequence, int maxSequence)
    {
      return this.CallAsync<IEnumerable<CoaterScheduleRoll>>("GetCoaterSchedule", minSequence, maxSequence);
    }

    public async Task GetServiceSettingsAsync(IServiceSettings serviceSettings)
    {
      // Clear the default values from the lists in serviceSetting
      // otherwise there will be duplicate values when populated from the server.
      //serviceSettings.BackingCodes.Clear();
      //serviceSettings.BackingSpecs.Clear();
      JObject obj = await this.CallAsync<JObject>("GetServiceSettings");
      obj.Populate(serviceSettings);
    }

    public void MoveQueueRoll(int rollIndex, int direction)
    {
      this.CallAsync(nameof(MoveQueueRoll), rollIndex, direction).NoWait();
    }

    private void HubConnection_StateChanged(StateChange obj)
    {
      TaskUtilities.RunOnMainThreadAsync(async () =>
      {
        this.IpcStatusMessage = obj.NewState.ToString();

        if (obj.NewState == ConnectionState.Connected)
        {
          this.connectionTcs.SetResult(null);
          this.CallAsync("RefreshAll").NoWait();
        }
        else if (obj.OldState == ConnectionState.Connected)
        {
          this.connectionTcs = new TaskCompletionSource<object>();
        }

        if (!this.isStarting && obj.NewState == ConnectionState.Disconnected && !this.IsDisposed)
        {
          await Task.Delay(5000);
          await this.StartAsync();
        }
      }).NoWait();
    }

    private static bool RetryCheck(string method, Exception ex)
    {
      var dr = MessageBox.Show("Try again?", $"Communication Failure ({method})\n{ex.Message}", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
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

      this.connectionError = builder.ToString();
    }

    /// <summary>
    /// Executes a method on the server side hub asynchronously.
    /// </summary>
    /// <param name="method">The name of the method.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>A task that represents when invocation returned.</returns>
    public Task CallAsync(string method, params object[] args)
    {
      return this.CallAsync<object>(method, args);
    }

    public async Task<T> CallAsync<T>(string method, params object[] args)
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
          result = await this.hubProxy.Invoke<T>(method, args);
          break;
        }
        catch (Exception ex) when (this.hubConnection.State != ConnectionState.Connected)
        {
          this.SetConnectionError(ex);
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
