using System.Threading.Tasks;

namespace MapperClient.Ipc
{
  interface IMahloClient
  {
    Task Start();
    Task Call(string cmd, params object[] args);
  }
}