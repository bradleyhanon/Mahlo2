using System;
using Mahlo.Opc;

namespace Mahlo.Logic
{
  interface IUserAttentions<Model>
  {
    bool Any { get; }
    IObservable<IUserAttentions<Model>> Changes { get; }
    bool IsRollTooLong { get; set; }
    bool IsRollTooShort { get; set; }
    bool IsSystemDisabled { get; set; }
    bool IsTimeToCheckRollSequence { get; set; }

    void ClearAll();
  }
}