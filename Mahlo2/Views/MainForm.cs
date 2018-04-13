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
      this.grFeetColumn.HeaderCell.Style.Alignment = 
        this.mahlo2FeetColumn.HeaderCell.Style.Alignment =
        this.bowAndSkewFeetColumn.HeaderCell.Style.Alignment =
        this.bowAndSkewBowColumn.HeaderCell.Style.Alignment =
        this.bowAndSkewSkewColumn.HeaderCell.Style.Alignment =
        this.patternRepeatFeetColumn.HeaderCell.Style.Alignment =
        this.patternRepeatElongationColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      this.carpetProcessor.Start();
      this.sewinGrid.DataSource = carpetProcessor.sewinQueue.Rolls;
      //this.mahlo2Grid.DataSource = carpetProcessor.mahloMeter.Rolls;
      //this.bowAndSkewGrid.DataSource = carpetProcessor.bowAndSkewMeter.Rolls;
      //this.patternRepeatGrid.DataSource = carpetProcessor.patternRepeatMeter.Rolls;
    }
  }
}
