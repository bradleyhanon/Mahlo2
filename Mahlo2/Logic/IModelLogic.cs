using System;
using System.Collections.Generic;
using System.Drawing;
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

    string PlcStatusMessage { get; }
    Color PlcStatusMessageBackColor { get; }
    Color PlcStatusMessageForeColor { get; }

    string MahloStatusMessage { get; }
    Color MahloStatusMessageBackColor { get; }
    Color MahloStatusMessageForeColor { get; }

    string MappingStatusMessage { get; }
    Color MappingStatusMessageBackColor { get; }
    Color MappingStatusMessageForeColor { get; }

    void MoveToNextRoll();
    void MoveToPriorRoll();
    void WaitForSeam();

    void Start();

  }
}
