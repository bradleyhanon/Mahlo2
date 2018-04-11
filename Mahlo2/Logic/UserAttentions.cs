using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Opc;

namespace Mahlo.Logic
{
  class UserAttentions
  {
    private UserAttentionEnum userAttentions;

    public IMahloSrc MahloSrc { get; set; }

    public bool IsSystemDisabled
    {
      get => (this.userAttentions & UserAttentionEnum.attSystemDisabled) != 0;
      set => this.SetUserAttention(UserAttentionEnum.attSystemDisabled, value);
    }

    public bool IsRollTooLong
    {
      get => (this.userAttentions & UserAttentionEnum.attRollTooLong) != 0;
      set => this.SetUserAttention(UserAttentionEnum.attRollTooLong, value);
    }

    public bool IsRollTooShort
    {
      get => (this.userAttentions & UserAttentionEnum.attRollTooShort) != 0;
      set => this.SetUserAttention(UserAttentionEnum.attRollTooShort, value);
    }

    public bool IsTimeToCheckRollSequence
    {
      get => (this.userAttentions & UserAttentionEnum.attVerifyRollSequence) != 0;
      set => this.SetUserAttention(UserAttentionEnum.attVerifyRollSequence, value);
    }

    public bool Any => this.userAttentions != 0;

    private void SetUserAttention(UserAttentionEnum AttentionValue, bool Off = false, bool ShowIndicator = true)
    {
      if (Off)
      {
        this.userAttentions &= ~AttentionValue;
      }
      else if ((userAttentions & AttentionValue) != AttentionValue)
      {
        userAttentions |= AttentionValue;

        switch (AttentionValue)
        {
          case UserAttentionEnum.attSystemDisabled:
            SetUserAttention(UserAttentionEnum.attVerifyRollSequence, false, false);
            break;

          case UserAttentionEnum.attRollTooLong:
            SetUserAttention(UserAttentionEnum.attRollTooShort, true, false);
            SetUserAttention(UserAttentionEnum.attVerifyRollSequence, false, false);
            break;

          case UserAttentionEnum.attRollTooShort:
            SetUserAttention(UserAttentionEnum.attRollTooLong, true, false);
            SetUserAttention(UserAttentionEnum.attVerifyRollSequence, false, false);
            break;
        }
      }

      if (ShowIndicator)
      {
        this.MahloSrc.SetStatusIndicator(this.userAttentions != 0);
      }
    }
  }
}
