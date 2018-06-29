using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;
using Newtonsoft.Json.Linq;
using PropertyChanged;

namespace MahloClient.Logic
{
  [AddINotifyPropertyChangedInterface]
  class CutRollList : BindingList<CutRoll>, ICutRollList
  {
    public void Update(JArray jArray)
    {
      int index = 0;
      foreach(JObject jObject in jArray)
      {
        if (index >= this.Count)
        {
          this.Add(new CutRoll());
        }

        jObject.Populate(this[index]);
        index++;
      }

      // Remove any extra entries
      while (index < this.Count)
      {
        this.RemoveAt(this.Count - 1);
      }
    }
  }
}
