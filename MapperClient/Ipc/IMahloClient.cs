using System.Collections.Generic;
using System.Threading.Tasks;
using Mahlo.Ipc;
using Mahlo.Models;

namespace MapperClient.Ipc
{
  interface IMahloClient
  {
    Task Start();
    Task Call(string cmd, params object[] args);
    Task<T> Call<T>(string cmd, params object[] args);
    Task<(string message, string caption)> BasSetRecipe(string rollNo, string styleCode, string recipeName, RecipeApplyToEnum applyTo);
    Task<IEnumerable<CoaterScheduleRoll>> GetCoaterSchedule(int minSequence, int maxSequence);
  }
}