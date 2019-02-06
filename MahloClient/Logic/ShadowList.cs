using System.ComponentModel;
using MahloService.Models;
using Newtonsoft.Json.Linq;
using PropertyChanged;

namespace MahloClient.Logic
{
  [AddINotifyPropertyChangedInterface]
  internal class ShadowList<T> : BindingList<T>, IShadowList<T>
    where T : class, new()
  {
    public void Update(JArray jArray)
    {
      int index = 0;
      foreach (JObject jObject in jArray)
      {
        if (index >= this.Count)
        {
          this.Add(new T());
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
