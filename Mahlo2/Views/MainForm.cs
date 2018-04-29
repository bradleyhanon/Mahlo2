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
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
        h => ((INotifyPropertyChanged)this.carpetProcessor.MahloLogic).PropertyChanged += h,
        h => ((INotifyPropertyChanged)this.carpetProcessor.MahloLogic).PropertyChanged -= h)
        .Where(args => args.EventArgs.PropertyName == nameof(this.carpetProcessor.MahloLogic.CurrentRoll))
        .Subscribe(args => this.mahloRollSrc.DataSource = this.carpetProcessor.MahloLogic.CurrentRoll);

      BowAndSkewPropertyChangedSubscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
        h => ((INotifyPropertyChanged)this.carpetProcessor.BowAndSkewLogic).PropertyChanged += h,
        h => ((INotifyPropertyChanged)this.carpetProcessor.BowAndSkewLogic).PropertyChanged -= h)
        .Where(args => args.EventArgs.PropertyName == nameof(this.carpetProcessor.BowAndSkewLogic.CurrentRoll))
        .Subscribe(args => this.bowAndSkewRollSrc.DataSource = this.carpetProcessor.BowAndSkewLogic.CurrentRoll);

      PatternRepeatPropertyChangedSubscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
        h => ((INotifyPropertyChanged)this.carpetProcessor.PatternRepeatLogic).PropertyChanged += h,
        h => ((INotifyPropertyChanged)this.carpetProcessor.PatternRepeatLogic).PropertyChanged -= h)
        .Where(args => args.EventArgs.PropertyName == nameof(this.carpetProcessor.PatternRepeatLogic.CurrentRoll))
        .Subscribe(args => this.patternRepeatRollSrc.DataSource = this.carpetProcessor.PatternRepeatLogic.CurrentRoll);

      // Make column heading alignment match column data alignment
      foreach (DataGridViewColumn column in dataGridView1.Columns)
      {
        column.HeaderCell.Style.Alignment = column.DefaultCellStyle.Alignment;
      }

      foreach (DataGridViewColumn column in dataGridView5.Columns)
      {
        column.HeaderCell.Style.Alignment = column.DefaultCellStyle.Alignment;
      }
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.MahloPropertyChangedSubscription.Dispose();
        this.BowAndSkewPropertyChangedSubscription.Dispose();
        this.PatternRepeatPropertyChangedSubscription.Dispose();
        this.components?.Dispose();
      }

      base.Dispose(disposing);
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
