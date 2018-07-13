﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Repository
{
  interface IProgramState
  {
    event Action<IProgramState> Saving;
    IProgramState GetSubState(params string[] names);
    T Get<T>(string name);
    void Set<T>(string name, T value);
    void RemoveAll();
    void Save();
  }
}
