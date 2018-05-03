using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;

namespace MapperClient.Logic
{
  class CriticalStops : ICriticalStops
  {
    public bool Any { get; set; }
    public bool IsMahloCommError { get; set; }
    public bool IsPlcCommError { get; set; }
  }
}
