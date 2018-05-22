using System.Collections.Generic;
using System.Threading.Tasks;
using MahloService.Ipc;
using MahloService.Models;

namespace MahloClient.Ipc
{
  interface IMahloIpcClient
  {
    Task Start();
    Task Call(string cmd, params object[] args);
    Task<T> Call<T>(string cmd, params object[] args);
    Task<(string message, string caption)> BasSetRecipe(string rollNo, string styleCode, string recipeName, RecipeApplyToEnum applyTo);
    Task<IEnumerable<CoaterScheduleRoll>> GetCoaterSchedule(int minSequence, int maxSequence);
  }
}