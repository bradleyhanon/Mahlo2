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
using Mahlo.Models;

namespace Mahlo.Views
{
  partial class MainForm : Form
  {
    // Yellowish
    private readonly Color ActiveBackColor = Color.FromArgb(255, 235, 156);
    private readonly Color ActiveForeColor = Color.FromArgb(156, 87, 0);

    // Pinkish background
    private readonly Color OutOfSpecBackColor = Color.FromArgb(255, 199, 206);
    private readonly Color OutOfSpecForeColor = Color.FromArgb(156, 0, 6); 

    private string[] mahloColumnNames = { nameof(CarpetRoll.MalFeet) };
    private string[] bowAndSkewColumnNames = { nameof(CarpetRoll.BasFeet), nameof(CarpetRoll.Bow), nameof(CarpetRoll.Skew) };
    private string[] patternRepeatColumnNames = { nameof(CarpetRoll.PrsFeet), nameof(CarpetRoll.Elongation) };

    private IDisposable MahloPropertyChangedSubscription;
    private IDisposable BowAndSkewPropertyChangedSubscription;
    private IDisposable PatternRepeatPropertyChangedSubscription;

    CarpetProcessor carpetProcessor;

    public MainForm(CarpetProcessor carpetProcessor)
    {
      InitializeComponent();
      this.carpetProcessor = carpetProcessor;

      this.MahloPropertyChangedSubscription = 
        this.carpetProcessor.MahloLogic.CurrentRollChanged.Subscribe(roll =>
        {
          this.mahloRollSrc.DataSource = roll;
          this.dataGridView1.Invalidate();
        });

      this.BowAndSkewPropertyChangedSubscription =
        this.carpetProcessor.BowAndSkewLogic.CurrentRollChanged.Subscribe(roll =>
        {
          this.bowAndSkewRollSrc.DataSource = roll;
          this.dataGridView1.Invalidate();
        });

      this.PatternRepeatPropertyChangedSubscription =
        this.carpetProcessor.PatternRepeatLogic.CurrentRollChanged.Subscribe(roll => 
        {
          this.patternRepeatRollSrc.DataSource = roll;
          this.dataGridView1.Invalidate();
        });

      // Line up the headings above the grid
      foreach (DataGridViewColumn column in dataGridView1.Columns)
      {
        if (column.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleRight)
        {
          column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        Label[] labels = { label1, label2, label3, label4 };

        int index = 0;
        int sum = -2;
        foreach (DataGridViewColumn col in this.dataGridView1.Columns)
        {
          sum += col.Width;
          if (string.IsNullOrWhiteSpace(col.DataPropertyName))
          {
            col.HeaderCell.Style.BackColor = SystemColors.AppWorkspace;
            if (index >= labels.Length)
            {
              break;
            }

            labels[index++].Width = sum - col.Width / 2 - 2;
            sum = col.Width - col.Width / 2 - 2;
          }
        }

        label4.Width = sum - 2;
      }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      this.carpetProcessor.Start();
      this.dataGridView1.DataSource = carpetProcessor.SewinQueue.Rolls;
      this.mahloRollSrc.DataSource = carpetProcessor.MahloLogic.CurrentRoll;
      this.bowAndSkewRollSrc.DataSource = carpetProcessor.BowAndSkewLogic.CurrentRoll;
      this.patternRepeatRollSrc.DataSource = carpetProcessor.PatternRepeatLogic.CurrentRoll;

      this.mahloLogicSrc.DataSource = carpetProcessor.MahloLogic;
      this.bowAndSkewLogicSrc.DataSource = carpetProcessor.BowAndSkewLogic;
      this.patternRepeatLogicSrc.DataSource = carpetProcessor.PatternRepeatLogic;

      this.grpMahlo.Tag = this.carpetProcessor.MahloLogic;
      this.grpBowAndSkew.Tag = this.carpetProcessor.BowAndSkewLogic;
      this.grpPatternRepeat.Tag = this.carpetProcessor.PatternRepeatLogic;
    }

    private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      var col = this.dataGridView1.Columns[e.ColumnIndex];
      e.Handled = string.IsNullOrWhiteSpace(col.DataPropertyName);
      if (e.Handled)
      {
        Rectangle rect = e.CellBounds;
        rect.X--;
        rect.Width++;
        e.Graphics.FillRectangle(SystemBrushes.AppWorkspace, rect);
      }
    }

    private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      var col = this.dataGridView1.Columns[e.ColumnIndex];
      CarpetRoll gridRoll = carpetProcessor.SewinQueue.Rolls[e.RowIndex];
      SetColor(this.carpetProcessor.MahloLogic.CurrentRoll, mahloColumnNames);
      SetColor(this.carpetProcessor.BowAndSkewLogic.CurrentRoll, bowAndSkewColumnNames);
      SetColor(this.carpetProcessor.PatternRepeatLogic.CurrentRoll, patternRepeatColumnNames);

      void SetColor(CarpetRoll currentRoll, string[] names)
      {
        if (currentRoll == gridRoll && names.Contains(col.DataPropertyName))
        {
          e.CellStyle.BackColor = ActiveBackColor;
          e.CellStyle.ForeColor = ActiveForeColor;
        }
      }
    }

    private void btnFore_Click(object sender, EventArgs e)
    {
      var btn = (Button)sender;
      var logic = (IModelLogic)btn.Parent.Tag;
      logic.MoveToNextRoll();
    }

    private void btnBack_Click(object sender, EventArgs e)
    {
      var btn = (Button)sender;
      var logic = (IModelLogic)btn.Parent.Tag;
      logic.MoveToPriorRoll();
    }

    private void btnWaitForSem_Click(object sender, EventArgs e)
    {
      var btn = (Button)sender;
      var logic = (IModelLogic)btn.Parent.Tag;
      logic.WaitForSeam();
    }
  }
}
