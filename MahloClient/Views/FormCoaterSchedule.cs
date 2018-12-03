using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MahloClient.Ipc;
using MahloService.Models;
using MahloService.Utilities;

namespace MahloClient.Views
{
  internal partial class FormCoaterSchedule : Form
  {
    private IMahloIpcClient mahloClient;
    private List<CoaterScheduleRoll> coaterSchedule = new List<CoaterScheduleRoll>();

    private int nMinSequence;
    private int nMaxSequence;

    public FormCoaterSchedule(IMahloIpcClient mahloClient)
    {
      this.InitializeComponent();

      this.mahloClient = mahloClient;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      TaskUtilities.RunOnMainThreadAsync(async () =>
      {
        this.ShowMessage("Loading schedule details...please wait");
        this.coaterSchedule = (await this.mahloClient.GetCoaterScheduleAsync(0, 500)).ToList();
        this.dbgCoaterSchedule.DataSource = this.coaterSchedule.Where(item => item.SeqNo >= 0 && item.SeqNo <= 99).ToList();
        this.LoadBackingSummary(this.coaterSchedule);
        this.ConfigureGrid();
        this.ShowMessage(string.Empty);
      }).NoWait(ex =>
      {
        MessageBox.Show($"Unable to get schedule.\n\n{ex.Message}", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        this.ShowMessage(string.Empty);
      });
    }

    private void LoadBackingSummary(IEnumerable<CoaterScheduleRoll> schedule)
    {
      var qry = from item in schedule
                group item by item.Backing into g
                orderby g.Key
                let totalMinutes = Math.Max(0, (int)g.Sum(item => item.Minutes))
                select new BackingSummaryItem
                {
                  Backing = g.Key,
                  Feet = (int)g.Sum(item => item.Feet),
                  Time = TimeSpan.FromMinutes(Math.Max(0, (double)g.Sum(item => item.Minutes))),
                };

      this.dbgBackingSummary.DataSource = qry.ToArray();
      this.dbgBackingSummary.Columns[0].Width = 70;
      this.dbgBackingSummary.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.dbgBackingSummary.Columns[1].Width = 70;
      this.dbgBackingSummary.Columns[2].Width = 70;
      this.dbgBackingSummary.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
    }

    private void ConfigureGrid()
    {
      for (int c = 0; c < this.dbgCoaterSchedule.Columns.Count; c++)
      {
        this.dbgCoaterSchedule.Columns[c].HeaderCell.Style.Alignment = this.dbgCoaterSchedule.Columns[c].DefaultCellStyle.Alignment;
      }
    }

    private void btnToggleView_Click(object sender, EventArgs e)
    {
      if (this.btnToggleView.Text == "Show Summary")
      {
        this.btnToggleView.Text = "Show Details";
        this.dbgCoaterSchedule.Visible = false;
        this.dbgBackingSummary.Visible = true;
      }
      else
      {
        this.btnToggleView.Text = "Show Summary";
        this.dbgCoaterSchedule.Visible = true;
        this.dbgBackingSummary.Visible = false;
      }
    }

    private void btnWhichOrders_Click(object sender, EventArgs e)
    {
      if (this.btnWhichOrders.Text == "All Orders")
      {
        this.btnWhichOrders.Text = "Scheduled Orders";
        this.nMinSequence = 0;
        this.nMaxSequence = 500;
      }
      else
      {
        this.btnWhichOrders.Text = "All Orders";
        this.nMinSequence = 0;
        this.nMaxSequence = 99;
      }

      var schedule = this.coaterSchedule.Where(item => item.SeqNo >= this.nMinSequence && item.SeqNo <= this.nMaxSequence);
      this.srcCoaterSchedule.DataSource = schedule.ToArray();
      this.LoadBackingSummary(schedule);
    }

    private void ShowMessage(string Message)
    {
      this.pnlMessage.Text = Message;
    }

    private class BackingSummaryItem
    {
      public string Backing { get; set; }
      public int Feet { get; set; }
      public TimeSpan Time { get; set; }
    }
  }
}
