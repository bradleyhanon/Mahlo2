using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mahlo.Ipc;
using Mahlo.Logic;
using Mahlo.Models;
using MapperClient.AppSettings;
using MapperClient.Logic;
using MapperClient.Views;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MapperClient.Ipc
{
  class MahloClient : IMahloClient
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

    public MahloClient(
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

    public async void BasSetRecipe(string rollNo, string styleCode, string recipeName, RecipeApplyToEnum applyTo)
    {
      await this.Call(nameof(BasSetRecipe), rollNo, styleCode, recipeName, applyTo);
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
    public async Task Call(string method, params object[] args)
    {
      Console.Clear();
      StringBuilder builder = new StringBuilder();
      builder.Append(method);
      builder.Append('(');
      foreach (var arg in args)
      {
        builder.Append(arg.ToString());
      }

      builder.Append(");");

      Console.WriteLine(builder.ToString());

      for (; ; )
      {
        try
        {
          await ConnectedCheck();
          await hubProxy.Invoke(method, args);
          break;
        }
        catch (Exception ex)
        {
          SetConnectionError(ex);
          if (!RetryCheck(method, ex))
          {
            break;
          }
        }
      }
    }
  }
}
