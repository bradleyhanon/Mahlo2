using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;
using PropertyChanged;

namespace MapperClient.Logic
{
  [AddINotifyPropertyChangedInterface]
  abstract class ModelLogic : IModelLogic
  {
    public CarpetRoll CurrentRoll { get; set; } = new CarpetRoll();

    public IObservable<CarpetRoll> RollStarted => throw new NotImplementedException();

    public IObservable<CarpetRoll> RollFinished => throw new NotImplementedException();

    public string PlcStatusMessage { get; set; }
    public Color PlcStatusMessageBackColor { get; set; }
    [DependsOn(nameof(PlcStatusMessageBackColor))]
    public Color PlcStatusMessageForeColor => PlcStatusMessageBackColor.ContrastColor();

    public string MahloStatusMessage { get; set; }
    public Color MahloStatusMessageBackColor { get; set; }
    [DependsOn(nameof(MahloStatusMessageBackColor))]
    public Color MahloStatusMessageForeColor => MahloStatusMessageBackColor.ContrastColor();

    public string MappingStatusMessage { get; set; }
    public Color MappingStatusMessageBackColor { get; set; }
    [DependsOn(nameof(MappingStatusMessageBackColor))]
    public Color MappingStatusMessageForeColor => MappingStatusMessageBackColor.ContrastColor();

    public void MoveToNextRoll()
    {
      throw new NotImplementedException();
    }

    public void MoveToPriorRoll()
    {
      throw new NotImplementedException();
    }

    public void Start()
    {
      throw new NotImplementedException();
    }

    public void WaitForSeam()
    {
      throw new NotImplementedException();
    }
  }
}
