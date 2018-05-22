using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.AppSettings
{
  class OpcSettings : IOpcSettings
  {
    public OpcSettings(IMahloOpcSettings mahloSettings, IPlcSettings seamSettings)
    {
      this.Mahlo = mahloSettings;
      this.Seam = seamSettings;
    }

    public IMahloOpcSettings Mahlo { get; }

    public IPlcSettings Seam { get; }
  }
}
