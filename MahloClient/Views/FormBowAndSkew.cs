using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
  internal partial class FormBowAndSkew : Form
  {
    private IBowAndSkewLogic logic;
    private ISewinQueue sewinQueue;
    private IMahloIpcClient mahloClient;
    private IServiceSettings serviceSettings;

    private List<IDisposable> disposables = new List<IDisposable>();

    public FormBowAndSkew(IBowAndSkewLogic logic, ISewinQueue sewinQueue, IMahloIpcClient mahloClient, IServiceSettings serviceSettings)
    {
      this.InitializeComponent();
      this.statusBar1.StatusBarInfo = (IStatusBarInfo)logic;
      this.logic = logic;
      this.sewinQueue = sewinQueue;
      this.mahloClient = mahloClient;
      this.serviceSettings = serviceSettings;

      this.disposables.AddRange(new[] {
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
        }),

        Observable.FromEventPattern<EventHandler, EventArgs>(
          h=>Application.Idle += h,
          h=>Application.Idle -= h)
        .Subscribe(args => this.btnSetRecipe.Enabled = this.dataGridView1.WideRowIndex >= 0),
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
        this.disposables.ForEach(item => item.Dispose());
        this.components?.Dispose();
      }

      base.Dispose(disposing);
    }

    private void BtnGoToPreviousRoll_Click(object sender, EventArgs e)
    {
      this.logic.MoveToPriorRoll();
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
      using (var form = new FormCoaterSchedule(this.mahloClient))
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

    private void BtnSetRecipe_Click(object sender, EventArgs e)
    {
      int wideRowIndex = this.dataGridView1.WideRowIndex;

      // Turn off "Auto Unwiden"
      this.dataGridView1.WideRowIndex = wideRowIndex;

      var selectedRoll = this.sewinQueue.Rolls[wideRowIndex];
      using (var form = new FormSetRecipe(this.mahloClient, this.logic.CurrentRoll, selectedRoll))
      {
        form.ShowDialog();
        this.dataGridView1.WideRowIndex = -1;
      }
    }

    private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      if (e.RowIndex >= 0)
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
        else if (roll.IsInLimbo)
        {
          (e.CellStyle.ForeColor, e.CellStyle.BackColor) = CellColor.GetLimboColor();
        }
      }
    }
  }
}