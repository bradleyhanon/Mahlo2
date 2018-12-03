using System;

namespace MahloService.Repository
{
  internal interface IProgramState
  {
    event Action<IProgramState> Saving;
    IProgramState GetSubState(params string[] names);
    T Get<T>(string name);
    void Set<T>(string name, T value);
    void RemoveAll();
    void Save();
  }
}
