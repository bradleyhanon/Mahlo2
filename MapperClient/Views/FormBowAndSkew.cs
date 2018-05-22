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
using MapperClient.Ipc;
using MapperClient.Logic;

namespace MapperClient.Views
{
  partial class FormBowAndSkew : Form
  {
    private ICarpetProcessor carpetProcessor;
    private IMahloClient mahloClient;

    private IDisposable BowAndSkewPropertyChangedSubscription;


    public FormBowAndSkew(ICarpetProcessor carpetProcessor, IMahloClient mahloClient)
    {
      InitializeComponent();
      this.carpetProcessor = carpetProcessor;
      this.mahloClient = mahloClient;

      BowAndSkewPropertyChangedSubscription =
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
      this.srcCurrentRoll.DataSource = this.carpetProcessor.BowAndSkewLogic.CurrentRoll;
      this.srcLogic.DataSource = this.carpetProcessor.BowAndSkewLogic;
    }

    private void BtnGoToPreviousRoll_Click(object sender, EventArgs e)
    {
      var dr = MessageBox.Show("You have requested to move back to the previous roll in the queue.  This will cancel mapping that may be in progress.\n\nAre you sure you want to do this?", "Alert!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dr == DialogResult.Yes)
      {
        this.mahloClient.Call(MahloClient.MoveToPriorRollCommand, nameof(IBowAndSkewLogic));
      }
    }

    private void BtnGoToNextRoll_Click(object sender, EventArgs e)
    {
      var dr = MessageBox.Show("You have requested to move ahead to the next roll in the queue.  This will cancel mapping that may be in progress.\n\nAre you sure you want to do this?", "Alert!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dr == DialogResult.Yes)
      {
        this.mahloClient.Call(MahloClient.MoveToNextRollCommand, nameof(IBowAndSkewLogic));
      }
    }

    private void BtnDisableSystem_Click(object sender, EventArgs e)
    {

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

    private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex == colDefaultRecipe.Index && e.RowIndex >= 0)
      {
        var selectedRoll = this.carpetProcessor.SewinQueue.Rolls[e.RowIndex];
        using (var form = new FormSetRecipe(this.mahloClient, this.carpetProcessor.BowAndSkewLogic.CurrentRoll, selectedRoll))
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
  }
}
