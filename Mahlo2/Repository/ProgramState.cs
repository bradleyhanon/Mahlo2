using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mahlo.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mahlo.Repository
{
  sealed class ProgramState : DynamicObject, IProgramState
  {
    IProgramStateProvider provider;
    JObject root;

    public ProgramState(IProgramStateProvider provider)
    {
      this.provider = provider;

      try
      {
        string text = provider.GetProgramState() ?? "{}";
        this.root = JObject.Parse(text);
      }
      catch (Exception)
      {
      }
    }

    public IDynamicMetaObjectProvider GetObject(params string[] names)
    {
      dynamic obj = this.root;
      foreach (var name in names)
      {
        obj[name] = obj[name] ?? JToken.FromObject(new { });
        obj = obj[name];
      }

      return obj;
    }

    public void Reset()
    {
      this.root.RemoveAll();
    }

    //public dynamic CreatePropertyBag()
    //{
    //  return new JObject();
    //}

    void IDisposable.Dispose()
    {
      provider.SaveProgramState(root.ToString());
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
      return this.root.Properties().Select(item => item.Name);
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      result = this.root[binder.Name];
      return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      this.root[binder.Name] = JToken.FromObject(value);
      return true;
    }

    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
    {
      string name = (string)indexes[0];
      result = this.root[name];
      return true; ;
    }

    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
    {
      string name = (string)indexes[0];
      this.root[name] = JToken.FromObject(value);
      return true;
    }

    //public class Container : DynamicObject
    //{
    //  private Dictionary<string, object> properties = new Dictionary<string, object>();

    //  public override bool TryGetMember(GetMemberBinder binder, out object result)
    //  {
    //    this.properties.TryGetValue(binder.Name, out result);
    //    return true;
    //  }

    //  public override bool TrySetMember(SetMemberBinder binder, object value)
    //  {
    //    this.properties[binder.Name] = value;
    //    return true;
    //  }

    //  public override IEnumerable<string> GetDynamicMemberNames()
    //  {
    //    return this.properties.Keys;
    //  }

    //  //public override bool TryConvert(ConvertBinder binder, out object result)
    //  //{
    //  //  Convert.ChangeType()
    //  //  binder.Type
    //  //  return base.TryConvert(binder, out result);
    //  //}
    //}
  }
}
