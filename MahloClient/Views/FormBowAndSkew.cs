﻿using System;
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
  partial class FormBowAndSkew : Form
  {
    private IBowAndSkewLogic logic;
    private ISewinQueue sewinQueue;
    private IMahloIpcClient mahloClient;

    private IDisposable BowAndSkewPropertyChangedSubscription;


    public FormBowAndSkew(IBowAndSkewLogic logic, ISewinQueue sewinQueue, IMahloIpcClient mahloClient)
    {
      InitializeComponent();
      this.statusBar1.StatusBarInfo = (IStatusBarInfo)logic;
      this.logic = logic;
      this.sewinQueue = sewinQueue;
      this.mahloClient = mahloClient;

      BowAndSkewPropertyChangedSubscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.logic).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.logic).PropertyChanged -= h)
        .Where(args => 
          args.EventArgs.PropertyName == nameof(this.logic.CurrentRoll))
        .Subscribe(args =>
        {
          this.srcCurrentRoll.DataSource = this.logic.CurrentRoll;
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
        this.BowAndSkewPropertyChangedSubscription?.Dispose();
      }

      base.Dispose(disposing);
    }

    private void BtnGoToPreviousRoll_Click(object sender, EventArgs e)
    {
      var dr = MessageBox.Show("You have requested to move back to the previous roll in the queue.  This will cancel mapping that may be in progress.\n\nAre you sure you want to do this?", "Alert!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dr == DialogResult.Yes)
      {
        this.mahloClient.Call(Ipc.MahloIpcClient.MoveToPriorRollCommand, nameof(IBowAndSkewLogic));
      }
    }

    private void BtnGoToNextRoll_Click(object sender, EventArgs e)
    {
      var dr = MessageBox.Show("You have requested to move ahead to the next roll in the queue.  This will cancel mapping that may be in progress.\n\nAre you sure you want to do this?", "Alert!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dr == DialogResult.Yes)
      {
        this.mahloClient.Call(Ipc.MahloIpcClient.MoveToNextRollCommand, nameof(IBowAndSkewLogic));
      }
    }

    private void BtnDisableSystem_Click(object sender, EventArgs e)
    {

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

    private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex == colDefaultRecipe.Index && e.RowIndex >= 0)
      {
        var selectedRoll = this.sewinQueue.Rolls[e.RowIndex];
        using (var form = new FormSetRecipe(this.mahloClient, this.logic.CurrentRoll, selectedRoll))
        {
          form.ShowDialog();
        }
      }
    }

    private void BtnViewCoaterSchedule_Click(object sender, EventArgs e)
    {
      using (var form = new FormCoaterSchedule(this.mahloClient))
      {
        form.ShowDialog();
      }
    }

    private void btnWaitForSeam_Click(object sender, EventArgs e)
    {

    }
  }
}
