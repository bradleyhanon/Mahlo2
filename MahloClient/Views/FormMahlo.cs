using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahloClient.Ipc;
using MahloClient.Logic;
using MahloService.Logic;

namespace MahloClient.Views
{
  partial class FormMahlo : Form
  {
    private ICarpetProcessor carpetProcessor;
    private IMahloIpcClient mahloClient;

    private IDisposable MahloPropertyChangedSubscription;

    public FormMahlo(ICarpetProcessor carpetProcessor, IMahloIpcClient mahloClient)
    {
      InitializeComponent();
      this.carpetProcessor = carpetProcessor;
      this.mahloClient = mahloClient;

      MahloPropertyChangedSubscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.carpetProcessor.BowAndSkewLogic).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.carpetProcessor.BowAndSkewLogic).PropertyChanged -= h)
        .Where(args =>
          args.EventArgs.PropertyName == nameof(this.carpetProcessor.BowAndSkewLogic.CurrentRoll))
        .Subscribe(args =>
        {
          this.srcCurrentRoll.DataSource = this.carpetProcessor.BowAndSkewLogic.CurrentRoll;
          this.DataGridView1_SelectionChanged(this.dataGridView1, EventArgs.Empty);
        });

      // Make column heading alignment match column data alignment
      foreach (DataGridViewColumn column in dataGridView1.Columns)
      {
        column.HeaderCell.Style.Alignment = column.DefaultCellStyle.Alignment;
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.srcGrid.DataSource = this.carpetProcessor.SewinQueue.Rolls;
      this.srcCurrentRoll.DataSource = this.carpetProcessor.MahloLogic.CurrentRoll;
      this.srcLogic.DataSource = this.carpetProcessor.MahloLogic;
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

    private void BtnGoToPreviousRoll_Click(object sender, EventArgs e)
    {
      var dr = MessageBox.Show("You have requested to move back to the previous roll in the queue.  This will cancel mapping that may be in progress.\n\nAre you sure you want to do this?", "Alert!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dr == DialogResult.Yes)
      {
        this.mahloClient.Call(Ipc.MahloIpcClient.MoveToPriorRollCommand, nameof(IMahloLogic));
      }
    }

    private void BtnGoToNextRoll_Click(object sender, EventArgs e)
    {
      var dr = MessageBox.Show("You have requested to move ahead to the next roll in the queue.  This will cancel mapping that may be in progress.\n\nAre you sure you want to do this?", "Alert!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dr == DialogResult.Yes)
      {
        this.mahloClient.Call(Ipc.MahloIpcClient.MoveToNextRollCommand, nameof(IMahloLogic));
      }
    }

    private void DataGridView1_SelectionChanged(object sender, EventArgs e)
    {
      int index = this.carpetProcessor.SewinQueue.Rolls.IndexOf(this.carpetProcessor.BowAndSkewLogic.CurrentRoll);
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
  }
}
