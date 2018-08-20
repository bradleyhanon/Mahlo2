using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Settings;
using MahloService.Ipc;
using MahloService.Models;
using MahloService.Opc;

namespace MahloService.Logic
{
  class CarpetProcessor : ICarpetProcessor
  {
    private readonly IOpcServerController opcServerController;
    private readonly IServiceSettings appInfo;

    public CarpetProcessor(
      ISewinQueue sewinQueue,
      IMahloLogic mahloLogic,
      IBowAndSkewLogic bowAndSkewLogic,
      IPatternRepeatLogic patternRepeatLogic,
      ICutRollLogic cutRollLogic,
      IOpcServerController opcServerController,
      IServiceSettings appInfo)
    {
      this.SewinQueue = sewinQueue;
      this.MahloLogic = mahloLogic;
      this.BowAndSkewLogic = bowAndSkewLogic;
      this.PatternRepeatLogic = patternRepeatLogic;
      this.CutRollLogic = cutRollLogic;
      this.opcServerController = opcServerController;
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
      this.opcServerController.Start();
      Startup.Start(this.appInfo.ServiceUrl);
      this.MahloLogic.Start();
      this.BowAndSkewLogic.Start();
      this.PatternRepeatLogic.Start();
    }

    public void Stop()
    {
    }
  }
}
