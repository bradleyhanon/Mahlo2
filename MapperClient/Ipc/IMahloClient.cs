using System.Threading.Tasks;
using Mahlo.Ipc;

namespace MapperClient.Ipc
{
  interface IMahloClient
  {
    Task Start();
    Task Call(string cmd, params object[] args);
    void BasSetRecipe(string rollNo, string styleCode, string recipeName, RecipeApplyToEnum applyTo);
  }
}