using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Models
{
  public class CoaterScheduleRoll
  {
    public int SeqNo { get; set; }
    public string SchedNo { get; set; }
    public string Style { get; set; }
    public string Color { get; set; }
    public string Backing { get; set; }
    public int CutLength { get; set; }
    public string Comment2 { get; set; }
    public string Sewnin { get; set; }
    public string Comment { get; set; }
    public decimal FaceWt { get; set; }
    public int Process { get; set; }
    public int Rolls { get; set; }
    public decimal Feet { get; set; }
    public decimal Minutes { get; set; }
    public int OrigSeq { get; set; }
    public DateTime Promised { get; set; }
    public string Rush { get; set; }
  }
}
