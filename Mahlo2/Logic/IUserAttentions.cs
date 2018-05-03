using System;

namespace Mahlo.Logic
{
  
  interface IUserAttentions
  {
    bool Any { get; }
    bool IsRollTooLong { get; set; }
    bool IsRollTooShort { get; set; }
    bool IsSystemDisabled { get; set; }
    bool VerifyRollSequence { get; set; }
  }

  interface IUserAttentions<Model> : IUserAttentions
  {
    void ClearAll();
  }
}