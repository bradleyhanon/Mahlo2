using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Ipc
{
  interface IMahloServer
  {
    void RefreshAll(string connectionId);
  }
}
