using System.Collections.Generic;
using Mahlo.Models;
using Newtonsoft.Json;

namespace Mahlo.Logic
{
  [JsonObject(MemberSerialization.OptIn)]
  interface IMahloLogic : IModelLogic
  {
  }
}