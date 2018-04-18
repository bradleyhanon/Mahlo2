using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace Mahlo.Repository
{
  interface IProgramState : IDynamicMetaObjectProvider, IDisposable
  {
    IDynamicMetaObjectProvider GetObject(params string[] names);
    void Reset();
  }
}
