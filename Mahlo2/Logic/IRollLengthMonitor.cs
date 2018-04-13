using Mahlo.Models;
using Mahlo.Opc;

namespace Mahlo.Logic
{
  interface IRollLengthMonitor<Model>
    where Model : MahloRoll
  {
    GreigeRoll CurrentGreigeRoll { get; set; }
    MahloRoll CurrentRoll { get; set; }
    IMeterSrc<Model> srcData { get; set; }

    void MetersCountChanged(int feetCount);
    void SeamDetected();
  }
}