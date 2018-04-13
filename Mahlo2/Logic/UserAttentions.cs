using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Opc;

namespace Mahlo.Logic
{
  class UserAttentions<Model> : IUserAttentions<Model>
  {
    private Attention attentions;
    private BehaviorSubject<IUserAttentions<Model>> changes;
    private IMeterSrc<Model> meterSrc;

    public UserAttentions(IMeterSrc<Model> meterSrc)
    {
      this.meterSrc = meterSrc;
      this.changes = new BehaviorSubject<IUserAttentions<Model>>(this);
    }

    [Flags]
    private enum Attention
    {
      VerifyRollSequence = 1,
      RollTooLong = 2,
      RollTooShort = 4,
      SystemDisabled = 8,
      All = VerifyRollSequence | RollTooLong | RollTooShort | SystemDisabled,
    }

    public IObservable<IUserAttentions<Model>> Changes => this.changes.AsObservable();

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

    public bool IsTimeToCheckRollSequence
    {
      get => (this.attentions & Attention.VerifyRollSequence) != 0;
      set => this.SetUserAttention(Attention.VerifyRollSequence, value);
    }

    public bool Any => this.attentions != 0;

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
        attentions |= (bitMask | Attention.VerifyRollSequence);

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

      if (this.attentions != oldValue)
      {
        this.changes.OnNext(this);
        this.meterSrc.SetStatusIndicator(this.attentions != 0);
      }
    }
  }
}
