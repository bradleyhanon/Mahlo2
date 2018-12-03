using System.ComponentModel;
using MahloService.Models;

namespace MahloService.Logic
{
  internal class CutRollList : BindingList<CutRoll>
  {
    public CutRollList()
    {
    }

    public bool IsChanged { get; set; } = true;

    protected override void OnListChanged(ListChangedEventArgs e)
    {
      base.OnListChanged(e);
      this.IsChanged = true;
    }
  }
}
