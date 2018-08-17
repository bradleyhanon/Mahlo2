using System.Threading.Tasks;

namespace MahloService.Utilities
{
  internal interface IOpcServerController
  {
    void Start();
    void Stop();
  }
}