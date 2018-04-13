using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Repository
{
  interface IProgramStateProvider
  {
    string GetProgramState();
    void SaveProgramState(string state);
  }
}
