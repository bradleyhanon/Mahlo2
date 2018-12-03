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
  internal interface IMahloIpcClient
  {
    event Action<string> IpcStatusMessageChanged;
    event Action<(string name, JObject jObject)> MeterLogicUpdated;
    ConnectionState State { get; }

    Task StartAsync();
    Task CallAsync(string cmd, params object[] args);
    Task<T> CallAsync<T>(string cmd, params object[] args);
    Task<(string message, string caption)> BasSetRecipeAsync(string rollNo, string styleCode, string recipeName, RecipeApplyToEnum applyTo);
    Task<IEnumerable<CoaterScheduleRoll>> GetCoaterScheduleAsync(int minSequence, int maxSequence);
    Task GetServiceSettingsAsync(IServiceSettings serviceSettings);
    void MoveQueueRoll(int rollIndex, int direction);
  }
}