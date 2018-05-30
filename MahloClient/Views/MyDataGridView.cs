using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MahloClient.Views
{
  class MyDataGridView : DataGridView
  {
    private int wideRowIndex = -1;
    private CancellationTokenSource cts;

    public event EventHandler WideRowIndexChanged;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int WideRowIndex
    {
      get => this.wideRowIndex;
      set
      {
        this.cts?.Cancel();
        if (this.wideRowIndex != value)
        {
          // Revert prior row
          if (this.wideRowIndex >= 0 && this.wideRowIndex < this.RowCount)
          {
            this.Rows[this.wideRowIndex].Height = this.RowTemplate.Height;
          }

          this.wideRowIndex = value;

          if (this.wideRowIndex >= 0 && this.wideRowIndex < this.RowCount)
          {
            this.Rows[this.wideRowIndex].Height = this.RowTemplate.Height * 2;
          }

          this.WideRowIndexChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }

    protected override async void OnCellClick(DataGridViewCellEventArgs e)
    {
      base.OnCellClick(e);

      this.WideRowIndex = this.WideRowIndex == e.RowIndex ? -1 : e.RowIndex;

      try
      {
        if (this.WideRowIndex >= 0)
        {
          this.cts = new CancellationTokenSource();
          await Task.Delay(5000, this.cts.Token);
          this.WideRowIndex = -1;
        }
      }
      catch (TaskCanceledException)
      {
      }
    }
  }
}
