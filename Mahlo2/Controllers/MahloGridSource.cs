using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Controllers
{
  class MahloGridSource : BindingList<MahloGridSource.MahloGridItem>
  {

    public MahloGridSource(ISewinQueue sewinQueue, IMahlo2  mahlo2, IMahloBowAndSkew bowAndSkew, IPatternRepeat patternRepeat)
    {

    }

    public class MahloGridItem
    {
      
    }
  }
}
