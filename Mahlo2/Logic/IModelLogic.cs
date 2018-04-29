using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

namespace Mahlo.Logic
{
  interface IModelLogic
  {
    CarpetRoll CurrentRoll { get; }
    IObservable<CarpetRoll> RollStarted { get; }
    IObservable<CarpetRoll> RollFinished { get; }

    void MoveToNextRoll();
    void MoveToPriorRoll();
    void WaitForSeam();

    void Start();

  }
}
