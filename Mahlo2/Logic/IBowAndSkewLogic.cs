﻿using Mahlo.Models;
using Newtonsoft.Json;

namespace Mahlo.Logic
{
  [JsonObject(MemberSerialization.OptIn)]
  interface IBowAndSkewLogic : IModelLogic
  {
  }
}