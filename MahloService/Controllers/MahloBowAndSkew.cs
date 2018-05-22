using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;
using Mahlo.Repository;

namespace Mahlo.Controllers
{
  class MahloBowAndSkew
  {
    public ObservableCollection<BowAndSkewRoll> rolls = new ObservableCollection<BowAndSkewRoll>();

    private int rollId;
    private int position;
    
    public MahloBowAndSkew(IDbLocal dbLocal)
    {

    }

    public void SetBow(float bow)
    {

    }

    public void SetSkew(float skew)
    {

    }

    public void SetNewRoll()
    {

    }
  }
}
