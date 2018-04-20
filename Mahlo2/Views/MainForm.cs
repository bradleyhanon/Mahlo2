using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mahlo.Logic;

namespace Mahlo.Views
{
  partial class MainForm : Form
  {
    CarpetProcessor carpetProcessor;

    public MainForm(CarpetProcessor carpetProcessor)
    {
      InitializeComponent();
      this.carpetProcessor = carpetProcessor;
      foreach (DataGridViewColumn column in dataGridView1.Columns)
      {
        if (column.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleRight)
        {
          column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        Label[] labels = { label1, label2, label3, label4 };

        int index = 0;
        int sum = -2;
        foreach(DataGridViewColumn col in this.dataGridView1.Columns)
        {
          sum += col.Width;
          if (string.IsNullOrWhiteSpace(col.DataPropertyName))
          {
            if (index >= labels.Length)
            {
              break;
            }

            labels[index++].Width = sum - col.Width / 2 - 2;
            sum = col.Width - col.Width / 2 - 2;
          }
        }

        label4.Width = sum - 2;

        var qry0 = this.dataGridView1.Columns.Cast<DataGridViewColumn>();
        var qry1 = qry0.TakeWhile(col => !string.IsNullOrWhiteSpace(col.DataPropertyName));
        //this.tableLayoutPanel1.Columns. = qry1.Sum(col => col.Width) + 5;
        //var qry2 = qry0.Skip(qry1.Count() + 1).TakeWhile(col => !string.IsNullOrWhiteSpace(col.DataPropertyName));
        //var qry3 = qry0.Skip(qry1.Count() + qry2.Count() + 1).TakeWhile(col=>!st)
        int key = 0;
        var qry = from col in qry0
                  group col by key;

      }


    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      this.carpetProcessor.Start();
      this.dataGridView1.DataSource = carpetProcessor.SewinQueue.Rolls;
      this.mahlo2Source.DataSource = carpetProcessor.MahloLogic.CurrentRoll;
      this.bowAndSkewSource.DataSource = carpetProcessor.BowAndSkewLogic.CurrentRoll;
      this.patternRepeatSource.DataSource = carpetProcessor.PatternRepeatLogic.CurrentRoll;
    }
  }
}
