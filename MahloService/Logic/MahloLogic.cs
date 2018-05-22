using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.AppSettings;
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

    public override bool IsMapValid
    {
      get => this.CurrentRoll.MalMapValid;
      set => this.CurrentRoll.MalMapValid = value;
    }
  }
}
