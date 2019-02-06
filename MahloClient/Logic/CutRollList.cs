using System.ComponentModel;
using MahloService.Models;
using Newtonsoft.Json.Linq;
using PropertyChanged;

namespace MahloClient.Logic
{
  [AddINotifyPropertyChangedInterface]
  internal class CutRollList : ShadowList<CutRoll>, ICutRollList
  {
  }
}
