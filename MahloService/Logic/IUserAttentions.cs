using System;
using System.ComponentModel;

namespace MahloService.Logic
{
  
  interface IUserAttentions : INotifyPropertyChanged
  {
    bool Any { get; }
    bool IsRollTooLong { get; set; }
    bool IsRollTooShort { get; set; }
    bool IsSystemDisabled { get; set; }
    bool VerifyRollSequence { get; set; }
    void ClearAll();
  }

  interface IUserAttentions<Model> : IUserAttentions
  {
  }
}