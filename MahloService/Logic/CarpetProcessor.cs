using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Settings;
using MahloService.Ipc;
using MahloService.Models;

namespace MahloService.Logic
{
  class CarpetProcessor : ICarpetProcessor
  {
    private IServiceSettings appInfo;

    public CarpetProcessor(
      ISewinQueue sewinQueue,
      IMahloLogic mahloLogic,
      IBowAndSkewLogic bowAndSkewLogic,
      IPatternRepeatLogic patternRepeatLogic,
      ICutRollLogic cutRollLogic,
      IServiceSettings appInfo)
    {
      this.SewinQueue = sewinQueue;
      this.MahloLogic = mahloLogic;
      this.BowAndSkewLogic = bowAndSkewLogic;
      this.PatternRepeatLogic = patternRepeatLogic;
      this.CutRollLogic = cutRollLogic;
      this.appInfo = appInfo;
    }

    public ISewinQueue SewinQueue { get; }
    public IMahloLogic MahloLogic { get; }
    public IBowAndSkewLogic BowAndSkewLogic { get; }
    public IPatternRepeatLogic PatternRepeatLogic { get; }
    public ICutRollLogic CutRollLogic { get; }

    public int startRollId;

    public void Start()
    {
      Startup.Start(appInfo.ServiceUrl);
      //this.mahloLogic.Start();
      //this.bowAndSkewLogic.Start();
      //this.patternRepeatLogic.Start();
    }
  }
}
