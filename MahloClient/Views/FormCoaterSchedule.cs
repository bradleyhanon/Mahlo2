using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahloService.Models;
using MahloClient.Ipc;
using MahloService.Utilities;

namespace MahloClient.Views
{
  partial class FormCoaterSchedule : Form
  {
    private IMahloIpcClient mahloClient;
    private List<CoaterScheduleRoll> coaterSchedule = new List<CoaterScheduleRoll>();

    private int nMinSequence;
    private int nMaxSequence;

    public FormCoaterSchedule(IMahloIpcClient mahloClient)
    {
      InitializeComponent();

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

      dbgBackingSummary.DataSource = qry.ToArray();
      dbgBackingSummary.Columns[0].Width = 70;
      dbgBackingSummary.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
      dbgBackingSummary.Columns[1].Width = 70;
      dbgBackingSummary.Columns[2].Width = 70;
      dbgBackingSummary.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
    }

    private void ConfigureGrid()
    {
      for (int c = 0; c < dbgCoaterSchedule.Columns.Count; c++)
      {
        dbgCoaterSchedule.Columns[c].HeaderCell.Style.Alignment = dbgCoaterSchedule.Columns[c].DefaultCellStyle.Alignment;
      }
    }

    private void btnToggleView_Click(object sender, EventArgs e)
    {
      if (btnToggleView.Text == "Show Summary")
      {
        btnToggleView.Text = "Show Details";
        dbgCoaterSchedule.Visible = false;
        dbgBackingSummary.Visible = true;
      }
      else
      {
        btnToggleView.Text = "Show Summary";
        dbgCoaterSchedule.Visible = true;
        dbgBackingSummary.Visible = false;
      }
    }

    private void btnWhichOrders_Click(object sender, EventArgs e)
    {
      if (btnWhichOrders.Text == "All Orders")
      {
        btnWhichOrders.Text = "Scheduled Orders";
        nMinSequence = 0;
        nMaxSequence = 500;
      }
      else
      {
        btnWhichOrders.Text = "All Orders";
        nMinSequence = 0;
        nMaxSequence = 99;
      }

      var schedule = this.coaterSchedule.Where(item => item.SeqNo >= nMinSequence && item.SeqNo <= nMaxSequence);
      this.srcCoaterSchedule.DataSource = schedule.ToArray();
      this.LoadBackingSummary(schedule);
    }

    private void ShowMessage(string Message)
    {
      pnlMessage.Text = Message;
    }

    private class BackingSummaryItem
    {
      public string Backing { get; set; }
      public int Feet { get; set; }
      public TimeSpan Time { get; set; }
    }
  }
}
