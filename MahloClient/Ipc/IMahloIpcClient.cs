using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MahloService.Ipc;
using MahloService.Models;
using MahloService.Settings;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;

namespace MahloClient.Ipc
{
  interface IMahloIpcClient
  {
    event Action<string> IpcStatusMessageChanged;
    event Action<(string name, JObject jObject)> MeterLogicUpdated;
    ConnectionState State { get; }

    Task Start();
    Task Call(string cmd, params object[] args);
    Task<T> Call<T>(string cmd, params object[] args);
    Task<(string message, string caption)> BasSetRecipe(string rollNo, string styleCode, string recipeName, RecipeApplyToEnum applyTo);
    Task<IEnumerable<CoaterScheduleRoll>> GetCoaterSchedule(int minSequence, int maxSequence);
    Task GetServiceSettings(IServiceSettings serviceSettings);
    void MoveQueueRoll(int rollIndex, int direction);
  }
}