using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Opc
{
  class OpcServerControllerSim : IOpcServerController
  {
    public void Start()
    {
      // We don't need to start the OPC server when we are simulating
    }
  }
}
