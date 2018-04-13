using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mahlo.Repository
{
  sealed class ProgramState : IProgramState
  {
    IProgramStateProvider provider;
    public ProgramState(IProgramStateProvider provider)
    {
      this.provider = provider;

      try
      {
        string text = provider.GetProgramState() ?? "{}";
        this.Root = JObject.Parse(text);
      }
      catch (Exception)
      {
      }
    }

    public dynamic Root { get; }

    void IDisposable.Dispose()
    {
      provider.SaveProgramState(Root.ToString());
    }

    public class Container : DynamicObject
    {
      private Dictionary<string, object> properties = new Dictionary<string, object>();

      public override bool TryGetMember(GetMemberBinder binder, out object result)
      {
        this.properties.TryGetValue(binder.Name, out result);
        return true;
      }

      public override bool TrySetMember(SetMemberBinder binder, object value)
      {
        this.properties[binder.Name] = value;
        return true;
      }

      public override IEnumerable<string> GetDynamicMemberNames()
      {
        return this.properties.Keys;
      }

      //public override bool TryConvert(ConvertBinder binder, out object result)
      //{
      //  Convert.ChangeType()
      //  binder.Type
      //  return base.TryConvert(binder, out result);
      //}
    }
  }
}
