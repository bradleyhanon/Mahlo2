using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

namespace Mahlo.Logic
{
  class CarpetProcessor
  {
    public CarpetProcessor(
      ISewinQueue sewinQueue,
      IMeterLogic<MahloRoll> mahloMeter,
      IMeterLogic<BowAndSkewRoll> bowAndSkewMeter,
      IMeterLogic<PatternRepeatRoll> patternRepeatMeter,
      ICutRollLogic cutRollLogic)
    {
      this.sewinQueue = sewinQueue;
      this.mahloMeter = mahloMeter;
      this.bowAndSkewMeter = bowAndSkewMeter;
      this.patternRepeatMeter = patternRepeatMeter;
      this.cutRollLogic = cutRollLogic;
    }

    public ISewinQueue sewinQueue { get; }
    public IMeterLogic<MahloRoll> mahloMeter { get; }
    public IMeterLogic<BowAndSkewRoll> bowAndSkewMeter { get; }
    public IMeterLogic<PatternRepeatRoll> patternRepeatMeter { get; }
    public ICutRollLogic cutRollLogic { get; }

    public int startRollId;

    public void Start()
    {
      this.mahloMeter.Start();
      this.bowAndSkewMeter.Start();
      this.patternRepeatMeter.Start();
    }
  }
}
