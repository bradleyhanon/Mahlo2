using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;
using MahloService.Repository;

namespace MahloService.Simulation
{
  class SimInfo : IDisposable
  {
    private readonly IDbMfgSim dbMfgSim;
    private readonly IProgramState programState;

    private int _malWebLength = 100;
    private int _basWebLength = 200;
    private int _prsWebLength = 200;
    private double _feetPerMinute = 60;

    public SimInfo(IDbMfgSim dbMfgSim, IProgramState programState)
    {
      this.dbMfgSim = dbMfgSim;
      this.programState = programState;
      this.SetCheckRollLength();

      this.FeetPerMinute = programState.GetSubState(nameof(SimInfo)).Get<double?>(nameof(FeetPerMinute)) ?? this.FeetPerMinute;
    }

    public double FeetPerMinute
    {
      get => _feetPerMinute;
      set => _feetPerMinute = Math.Max(60, value);
    }
    public int MalWebLength
    {
      get => _malWebLength;
      set
      {
        _malWebLength = value;
        this.SetCheckRollLength();
      }
    }

    public int BasWebLength
    {
      get => _basWebLength;
      set
      {
        _basWebLength = value;
        this.SetCheckRollLength();
      }
    }

    public int PrsWebLength
    {
      get => _prsWebLength;
      set
      {
        _prsWebLength = value;
        this.SetCheckRollLength();
      }
    }

    public void Dispose()
    {
      this.programState.Set(nameof(SimInfo), new { this.FeetPerMinute });
    }

    private void SetCheckRollLength()
    {
      this.dbMfgSim.SewinQueue
        .Where(roll => roll.IsCheckRoll)
        .ForEach(roll => roll.RollLength = this.MalWebLength + this.BasWebLength + this.PrsWebLength);
    }
  }
}
