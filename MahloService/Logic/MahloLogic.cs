using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Settings;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Utilities;
using Newtonsoft.Json;

namespace MahloService.Logic
{
  class MahloLogic : MeterLogic<MahloRoll>, IMahloLogic
  {
    public MahloLogic(
      IDbLocal dbLocal,
      IMahloSrc mahloSrc, 
      ISewinQueue sewinQueue, 
      IServiceSettings appInfo, 
      IUserAttentions<MahloRoll> userAttentions, 
      ICriticalStops<MahloRoll> criticalStops, 
      IProgramState programState,
      ISchedulerProvider schedulerProvider)
      : base(dbLocal, mahloSrc, sewinQueue, appInfo, userAttentions, criticalStops, programState, schedulerProvider)
    {
    }

    public override int FeetCounterStart 
    {
      get => this.CurrentRoll.MalFeetCounterStart;
      set => this.CurrentRoll.MalFeetCounterStart = value;
    }

    public override int FeetCounterEnd
    {
      get => this.CurrentRoll.MalFeetCounterEnd;
      set => this.CurrentRoll.MalFeetCounterEnd = value;
    }

    public override int Speed
    {
      get => this.CurrentRoll.MalSpeed;
      set => this.CurrentRoll.MalSpeed = value;
    }

    public override bool IsMapValid
    {
      get => this.CurrentRoll.MalMapValid;
      set => this.CurrentRoll.MalMapValid = value;
    }
  }
}
