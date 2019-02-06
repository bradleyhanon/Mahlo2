using System.Collections.Generic;
using System.ComponentModel;
using MahloService.Models;
using Newtonsoft.Json.Linq;

namespace MahloClient.Logic
{
  internal interface IShadowList<T> : IList<T>, IBindingList
    where T : class, new()
  {
    void Update(JArray jArray);
  }
}
