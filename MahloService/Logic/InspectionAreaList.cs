using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;

namespace MahloService.Logic
{
  class InspectionAreaList : BindingList<InspectionAreaDatum>
  {
    public bool IsChanged { get; set; } = true;

    protected override void OnListChanged(ListChangedEventArgs e)
    {
      base.OnListChanged(e);
      this.IsChanged = true;
    }
  }
}
