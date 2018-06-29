using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;
using Newtonsoft.Json.Linq;

namespace MahloClient.Logic
{
  interface ICutRollList : IList<CutRoll>, IBindingList
  {
    void Update(JArray jArray);
  }
}
