﻿namespace Mahlo.Logic
{
  interface ICarpetProcessor
  {
    IBowAndSkewLogic BowAndSkewLogic { get; }
    ICutRollLogic CutRollLogic { get; }
    IMahloLogic MahloLogic { get; }
    IPatternRepeatLogic PatternRepeatLogic { get; }
    ISewinQueue SewinQueue { get; }

    void Start();
  }
}