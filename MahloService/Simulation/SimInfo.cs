using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Simulation
{
  class SimInfo
  {
    private IDbMfgSim dbMfgSim;

    private int _malWebLength = 100;
    private int _basWebLength = 200;
    private int _prsWebLength = 200;
    private double _feetPerMinute = 60;

    public SimInfo(IDbMfgSim dbMfgSim)
    {
      this.dbMfgSim = dbMfgSim;
      this.SetCheckRollLength();
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

    private void SetCheckRollLength()
    {
      this.dbMfgSim.SewinQueue
        .Where(roll => roll.IsCheckRoll)
        .ForEach(roll => roll.RollLength = this.MalWebLength + this.BasWebLength + this.PrsWebLength);
    }
  }
}
