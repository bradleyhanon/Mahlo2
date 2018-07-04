using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MahloService.Repository
{
  class ProgramState : IProgramState
  {
    private readonly IProgramStateProvider provider;
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
        this.root = new JObject();
      }
    }

    private ProgramState(JObject root)
    {
      this.root = root;
    }

    public void Dispose()
    {
      this.provider?.SaveProgramState(root.ToString());
    }

    public IProgramState GetSubState(params string[] names)
    {
      JObject obj = this.root;
      foreach (var name in names)
      {
        obj[name] = obj[name] ?? new JObject();
        obj = (JObject)obj[name];
      }

      return new ProgramState(obj);
    }

    //public T GetObject<T>(string name)
    //  where T : class, new()
    //{
    //  JObject obj = (JObject)this.root[name];
    //  return obj.ToObject<T>();
    //}

    public T Get<T>(string name)
    {
      var obj = this.root[name];
      return obj == null ? default(T) : obj.ToObject<T>();
      //return root.Value<T>();
    }

    public void Set<T>(string name, T value)
    {
      root[name] = JToken.FromObject(value);
    }

    public void RemoveAll()
    {
      root.RemoveAll();
    }
  }
}
