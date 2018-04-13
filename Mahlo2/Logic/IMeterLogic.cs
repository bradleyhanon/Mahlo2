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
    GreigeRoll CurrentGreigeRoll { get; }
    Model CurrentRoll { get; }
    IObservable<Model> RollStarted { get; }
    IObservable<Model> RollFinished { get; }

    void Start();
  }
}
