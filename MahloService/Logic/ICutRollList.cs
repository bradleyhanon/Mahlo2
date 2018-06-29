﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;

namespace MahloService.Logic
{
  interface ICutRollList : IList<CutRoll>, IBindingList
  {
    bool IsChanged { get; set; }
  }
}
