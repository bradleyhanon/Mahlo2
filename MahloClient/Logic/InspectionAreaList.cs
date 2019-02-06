using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;
using PropertyChanged;

namespace MahloClient.Logic
{
  [AddINotifyPropertyChangedInterface]
  class InspectionAreaList : ShadowList<InspectionAreaDatum>, IInspectionAreaList
  {
  }
}
