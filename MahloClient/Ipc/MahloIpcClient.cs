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

namespace MahloClient.Ipc
{
  class MahloIpcClient : IMahloIpcClient
  {
    public const string MoveToNextRollCommand = "MoveToNextRoll";
    public const string MoveToPriorRollCommand = "MoveToPriorRoll";
    public const string WaitForSeamCommand = "WaitForSeam";

    private HubConnection hubConnection;
    private IHubProxy hubProxy;
    private IAppInfo appInfo;
    private ISewinQueue sewinQueue;
    private IMahloLogic mahloLogic;
    private IBowAndSkewLogic bowAndSkewLogic;
    private IPatternRepeatLogic patternRepeatLogic;
    private SynchronizationContext context;

    private bool isStarting;
    private ConnectionState state = ConnectionState.Disconnected;
    private string connectionError;

    public MahloIpcClient(
      ISewinQueue sewinQueue,
      IMahloLogic mahloLogic,
      IBowAndSkewLogic bowAndSkewLogic,
      IPatternRepeatLogic patternRepeatLogic,
      IAppInfo appInfo,
      SynchronizationContext context)
    {
      this.appInfo = appInfo;
      this.sewinQueue = sewinQueue;
      this.mahloLogic = mahloLogic;
      this.bowAndSkewLogic = bowAndSkewLogic;
      this.patternRepeatLogic = patternRepeatLogic;
      this.context = context;
    }

    public async Task Start()
    {
      if (!isStarting)
      {
        this.isStarting = true;

        this.hubConnection = new HubConnection(appInfo.ServerUrl);
        this.hubConnection.StateChanged += HubConnection_StateChanged;
        //this.hubConnection.TraceLevel = TraceLevels.All;
        //this.hubConnection.TraceWriter = Console.Out;
        this.hubProxy = hubConnection.CreateHubProxy("MahloHub");
        this.hubProxy.On("UpdateSewinQueue", arg =>
        {
          this.context.Post(_ =>
            this.sewinQueue.UpdateSewinQueue(arg.ToObject<CarpetRoll[]>()), null);
        });

        this.hubProxy.On("UpdateMahloLogic", arg => this.context.Post(_ =>
        {
          JToken token = arg;
          token.Populate(this.mahloLogic);
          this.mahloLogic.RefreshStatusDisplay();
          Console.WriteLine($"UpdateMahloLogic - {arg}");
        }, null));

        this.hubProxy.On("UpdateBowAndSkewLogic", arg => this.context.Post(_ =>
        {
          JToken token = arg;
          token.Populate(this.bowAndSkewLogic);
          this.bowAndSkewLogic.RefreshStatusDisplay();
        }, null));

        this.hubProxy.On("UpdatePatternRepeatLogic", arg => this.context.Post(_ =>
        {
          JToken token = arg;
          token.Populate(this.patternRepeatLogic);
          this.patternRepeatLogic.RefreshStatusDisplay();
        }, null));

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
            await Call("RefreshAll");
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

    public Task<(string message, string caption)> BasSetRecipe(string rollNo, string styleCode, string recipeName, RecipeApplyToEnum applyTo)
    {
      bool isManualMode = recipeName == FormSetRecipe.ManualModeRecipeName;
      return this.Call<(string, string)>(nameof(BasSetRecipe), rollNo, styleCode, recipeName, isManualMode, applyTo);
    }

    public Task<IEnumerable<CoaterScheduleRoll>> GetCoaterSchedule(int minSequence, int maxSequence)
    {
      return this.Call<IEnumerable<CoaterScheduleRoll>>(nameof(GetCoaterSchedule), minSequence, maxSequence);
    }

    private void HubConnection_StateChanged(StateChange obj)
    {
      this.state = obj.NewState;
    }

    private async Task ConnectedCheck()
    {
      if (this.state != ConnectionState.Connected)
      {
        await Start();
      }
    }

    private bool RetryCheck(string method, Exception ex)
    {
      var dr = MessageBox.Show("Try again?", "Communication Failure", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
      return dr == DialogResult.Retry;
      //var nav = (FreshNavigationContainer)App.Current.MainPage;
      //var currentPage = (FreshBaseContentPage)nav.CurrentPage;
      //return await currentPage.DisplayAlert($"Communication Failure", "Try Again?", "Yes", "No");
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
      Console.Clear();
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

      T result = default(T);
      for (; ; )
      {
        try
        {
          await ConnectedCheck();
          result = await hubProxy.Invoke<T>(method, args);
          break;
        }
        catch (Exception ex) when (this.state != ConnectionState.Connected)
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
