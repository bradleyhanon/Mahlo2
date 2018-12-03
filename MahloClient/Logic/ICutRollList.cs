using System.Collections.Generic;
using System.ComponentModel;
using MahloService.Models;
using Newtonsoft.Json.Linq;

namespace MahloClient.Logic
{
  internal interface ICutRollList : IList<CutRoll>, IBindingList
  {
    void Update(JArray jArray);
  }
}
