using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

namespace Mahlo.Logic
{
  interface IMeterLogic<Model> : IDisposable
  {
    CarpetRoll CurrentRoll { get; }
    IObservable<CarpetRoll> RollStarted { get; }
    IObservable<CarpetRoll> RollFinished { get; }

    void Start();
  }
}
