using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.AppSettings;
using Mahlo.Models;
using Mahlo.Opc;
using Mahlo.Repository;
using Mahlo.Utilities;

namespace Mahlo.Logic
{
  class MahloLogic : MeterLogic<MahloRoll>, IMahloLogic
  {
    public MahloLogic(
      IMeterSrc<MahloRoll> mahloSrc, 
      ISewinQueue sewinQueue, 
      IAppInfoBAS appInfo, 
      IUserAttentions<MahloRoll> userAttentions, 
      ICriticalStops<MahloRoll> criticalStops, 
      IProgramState programState,
      ISchedulerProvider schedulerProvider)
      : base(mahloSrc, sewinQueue, appInfo, userAttentions, criticalStops, programState, schedulerProvider)
    {
    }

    public override int Feet
    {
      get => this.CurrentRoll.MalFeet;
      set => this.CurrentRoll.MalFeet = value;
    }

    public override int Speed
    {
      get => this.CurrentRoll.MalSpeed;
      set => this.CurrentRoll.MalSpeed = value;
    }
  }
}
