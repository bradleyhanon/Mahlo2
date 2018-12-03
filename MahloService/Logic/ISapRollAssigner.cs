using MahloService.Models;

namespace MahloService.Logic
{
  internal interface ISapRollAssigner
  {
    void AssignSapRollTo(CutRoll cutRoll);
  }
}