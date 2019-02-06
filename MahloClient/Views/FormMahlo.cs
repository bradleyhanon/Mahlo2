using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using MahloClient.Ipc;
using MahloClient.Logic;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Settings;

namespace MahloClient.Views
{
  internal partial class FormMahlo : Form
  {
    private readonly string UpArrowStr = new string((char)0xdd, 1);
    private readonly string DownArrorStr = new string((char)0xde, 1);
    private IMahloLogic logic;
    private ISewinQueue sewinQueue;
    private IMahloIpcClient ipcClient;
    private IServiceSettings serviceSettings;
    private Font ArrowFont = new Font("Wingdings", 30);

    private IDisposable MahloPropertyChangedSubscription;

    public FormMahlo(IMahloLogic logic, ISewinQueue sewinQueue, IMahloIpcClient ipcClient, IServiceSettings serviceSettings)
    {
      this.InitializeComponent();
      this.logic = logic;
      this.sewinQueue = sewinQueue;
      this.ipcClient = ipcClient;
      this.serviceSettings = serviceSettings;
      this.statusBar1.StatusBarInfo = (IStatusBarInfo)this.logic;

      this.MahloPropertyChangedSubscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.logic).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.logic).PropertyChanged -= h)
        .Where(args =>
          args.EventArgs.PropertyName == nameof(this.logic.CurrentRoll))
        .Subscribe(args =>
        {
          this.srcCurrentRoll.DataSource = this.logic.CurrentRoll;
          int nextIndex = this.sewinQueue.Rolls.IndexOf(this.logic.CurrentRoll) + 1;
          this.srcNextRoll.DataSource = nextIndex < this.sewinQueue.Rolls.Count ? this.sewinQueue.Rolls[nextIndex] : new GreigeRoll();
          this.DataGridView1_SelectionChanged(this.dataGridView1, EventArgs.Empty);
          this.dataGridView1.EnsureVisibleRow(this.logic.CurrentRollIndex);
          this.myScrollBar1.AutoScrollPosition = this.logic.CurrentRollIndex - 2;
        });

      // Make column heading alignment match column data alignment
      foreach (DataGridViewColumn column in this.dataGridView1.Columns)
      {
        column.HeaderCell.Style.Alignment = column.DefaultCellStyle.Alignment;
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.srcGrid.DataSource = this.sewinQueue.Rolls;
      this.srcCurrentRoll.DataSource = this.logic.CurrentRoll;
      this.srcLogic.DataSource = this.logic;
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.components?.Dispose();
        this.MahloPropertyChangedSubscription?.Dispose();
      }

      base.Dispose(disposing);
    }

    private async void BtnGoToPreviousRoll_Click(object sender, EventArgs e)
    {
      await this.ipcClient.CallAsync(Ipc.MahloIpcClient.MoveToPriorRollCommand, nameof(IMahloLogic));
    }

    private void BtnGoToNextRoll_Click(object sender, EventArgs e)
    {
      using (var dlg = new MoveToNextDialog
      {
        RollNumber = this.logic.CurrentRoll.RollNo,
        RollLength = (int)this.logic.MeasuredLength,
        MaxLength = (int)this.logic.MeasuredLength,
      })
      {
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          this.logic.MoveToNextRoll(dlg.RollLength);
        }
      }
    }

    private void DataGridView1_SelectionChanged(object sender, EventArgs e)
    {
      int index = this.sewinQueue.Rolls.IndexOf(this.logic.CurrentRoll);
      if (index < 0)
      {
        this.dataGridView1.ClearSelection();
      }
      else
      {
        this.dataGridView1.Rows[index].Selected = true;
      }
    }

    private void BtnViewCoaterSchedule_Click(object sender, EventArgs e)
    {
      using (var form = new FormCoaterSchedule(this.ipcClient))
      {
        form.ShowDialog();
      }
    }

    private void BtnWaitForSeam_Click(object sender, EventArgs e)
    {
      this.logic.WaitForSeam();
    }

    private void BtnDisableSystem_Click(object sender, EventArgs e)
    {
      this.logic.DisableSystem();
    }

    private void DataGridView1_WideRowIndexChanged(object sender, EventArgs e)
    {
      this.moveUpColumn.Visible = this.moveDownColumn.Visible = this.dataGridView1.WideRowIndex >= 0;
    }

    private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex == this.dataGridView1.WideRowIndex)
      {
        if (e.ColumnIndex == this.moveUpColumn.Index)
        {
          this.ipcClient.MoveQueueRoll(e.RowIndex, -1);
        }
        else if (e.ColumnIndex == this.moveDownColumn.Index)
        {
          this.ipcClient.MoveQueueRoll(e.RowIndex, 1);
        }
      }
    }

    private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      if (e.RowIndex == this.dataGridView1.WideRowIndex)
      {
        string s =
          e.ColumnIndex == this.moveUpColumn.Index ? this.UpArrowStr :
          e.ColumnIndex == this.moveDownColumn.Index ? this.DownArrorStr : string.Empty;

        if (!string.IsNullOrEmpty(s))
        {
          var cell = this.dataGridView1[e.ColumnIndex, e.RowIndex];
          int selectedRowIndex = this.dataGridView1.SelectedCells.Cast<DataGridViewCell>().FirstOrDefault()?.RowIndex ?? -100;
          var foreColor = selectedRowIndex == this.dataGridView1.WideRowIndex ? e.CellStyle.SelectionForeColor : e.CellStyle.ForeColor;
          e.Paint(e.ClipBounds, e.PaintParts & ~DataGridViewPaintParts.ContentForeground);
          using (var brush = new SolidBrush(foreColor))
          {
            var stringFormat = new StringFormat
            {
              Alignment = StringAlignment.Center
            };

            e.Graphics.DrawString(s, this.ArrowFont, brush, e.CellBounds, stringFormat);
          }

          e.Handled = true;
        }
      }
    }

    private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      if (e.RowIndex >= 0)
      {
        {
          GreigeRoll roll = this.sewinQueue.Rolls[e.RowIndex];
          if (e.ColumnIndex == this.colMeasuredLength.Index)
          {
            if (e.RowIndex < this.logic.CurrentRollIndex)
            {
              (e.CellStyle.ForeColor, e.CellStyle.BackColor) =
                roll.RollLength == 0 ?
                new CellColor { ForeColor = this.dataGridView1.DefaultCellStyle.ForeColor, BackColor = this.dataGridView1.DefaultCellStyle.BackColor } :
                CellColor.GetFeetColor(roll.RollLength, (long)e.Value, this.serviceSettings);
            }
          }
          else if (roll.IsComplete)
          {
            (e.CellStyle.ForeColor, e.CellStyle.BackColor) = CellColor.IsCompletedColor;
          }
        }
      }
    }
  }
}