using System.Threading.Tasks;
using MahloService.Models;

namespace MahloService.Logic
{
  interface ISapRollAssigner
  {
    void AssignSapRollTo(CutRoll cutRoll);
  }
}