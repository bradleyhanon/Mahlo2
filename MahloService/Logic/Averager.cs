using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Logic
{
  class Averager
  {
    public int Count { get; private set; }
    public double Sum { get; private set; }
    public double Average => Sum / Count;

    public void Add(double value)
    {
      this.Sum += value;
      this.Count++;
    }

    public void Clear()
    {
      this.Sum = 0;
      this.Count = 0;
    }
  }
}
