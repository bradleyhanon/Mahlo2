using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;

namespace MapperClient.Logic
{
  class UserAttentions : IUserAttentions
  {
    public bool Any { get; set; }

    public bool IsRollTooLong { get; set; }
    public bool IsRollTooShort { get; set; }
    public bool IsSystemDisabled { get; set; }
    public bool VerifyRollSequence { get; set; }
  }
}
