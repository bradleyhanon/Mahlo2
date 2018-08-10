using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace MahloService.Logic
{
  [AddINotifyPropertyChangedInterface]
  class UserAttentions<Model> : IUserAttentions<Model>
  {
    private Attention attentions;

    [Flags]
    private enum Attention
    {
      VerifyRollSequence = 1,
      RollTooLong = 2,
      RollTooShort = 4,
      SystemDisabled = 8,
      All = VerifyRollSequence | RollTooLong | RollTooShort | SystemDisabled,
    }

    public bool IsSystemDisabled
    {
      get => (this.attentions & Attention.SystemDisabled) != 0;
      set => this.SetUserAttention(Attention.SystemDisabled, value);
    }

    public bool IsRollTooLong
    {
      get => (this.attentions & Attention.RollTooLong) != 0;
      set => this.SetUserAttention(Attention.RollTooLong, value);
    }

    public bool IsRollTooShort
    {
      get => (this.attentions & Attention.RollTooShort) != 0;
      set => this.SetUserAttention(Attention.RollTooShort, value);
    }

    public bool VerifyRollSequence
    {
      get => (this.attentions & Attention.VerifyRollSequence) != 0;
      set => this.SetUserAttention(Attention.VerifyRollSequence, value);
    }

    public bool Any { get; private set; }

    public void ClearAll()
    {
      this.SetUserAttention(Attention.All, false);
    }

    private void SetUserAttention(Attention bitMask, bool state)
    {
      var oldValue = this.attentions;
      if (!state)
      {
        this.attentions &= ~bitMask;
      }
      else
      {
        this.attentions |= (bitMask | Attention.VerifyRollSequence);

        switch (bitMask)
        {
          case Attention.RollTooLong:
            this.attentions &= ~(Attention.RollTooShort);
            break;

          case Attention.RollTooShort:
            this.attentions &= ~(Attention.RollTooLong);
            break;
        }
      }

      this.Any = this.attentions != 0;
    }
  }
}
