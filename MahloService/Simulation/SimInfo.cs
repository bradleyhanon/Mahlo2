using System;
using System.Linq;
using MahloService.Repository;

namespace MahloService.Simulation
{
  internal sealed class SimInfo : IDisposable
  {
    private readonly IDbMfgSim dbMfgSim;
    private readonly IProgramState programState;

    private int malWebLength = 100;
    private int basWebLength = 200;
    private int prsWebLength = 200;
    private double feetPerMinute = 60;

    public SimInfo(IDbMfgSim dbMfgSim, IProgramState programState)
    {
      this.dbMfgSim = dbMfgSim;
      this.programState = programState;
      this.SetCheckRollLength();

      this.FeetPerMinute = programState.GetSubState(nameof(SimInfo)).Get<double?>(nameof(this.FeetPerMinute)) ?? this.FeetPerMinute;
    }

    public double FeetPerMinute
    {
      get => this.feetPerMinute;
      set => this.feetPerMinute = Math.Max(60, value);
    }
    public int MalWebLength
    {
      get => this.malWebLength;
      set
      {
        this.malWebLength = value;
        this.SetCheckRollLength();
      }
    }

    public int BasWebLength
    {
      get => this.basWebLength;
      set
      {
        this.basWebLength = value;
        this.SetCheckRollLength();
      }
    }

    public int PrsWebLength
    {
      get => this.prsWebLength;
      set
      {
        this.prsWebLength = value;
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
