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
using MahloService.Logic;
using MahloService.Models;
using MahloClient.Ipc;
using MahloClient.Logic;
using MahloService.Settings;

namespace MahloClient.Views
{
  partial class MainForm : Form
  {
    delegate void CellFormattingAction(DataGridViewCellFormattingEventArgs args, IServiceSettings settings);

    private string[] mahloColumnNames = { nameof(GreigeRoll.MalFeet) };
    private string[] bowAndSkewColumnNames = { nameof(GreigeRoll.BasFeet), nameof(GreigeRoll.Bow), nameof(GreigeRoll.Skew) };
    private string[] patternRepeatColumnNames = { nameof(GreigeRoll.PrsFeet), nameof(GreigeRoll.Elongation) };

    private IDisposable MahloPropertyChangedSubscription;
    private IDisposable BowAndSkewPropertyChangedSubscription;
    private IDisposable PatternRepeatPropertyChangedSubscription;

    private ICarpetProcessor carpetProcessor;
    private ICutRollList cutRollList;
    private IMahloIpcClient mahloClient;
    private IServiceSettings serviceSettings;

    public MainForm(ICarpetProcessor carpetProcessor, ICutRollList cutRollList, IMahloIpcClient mahloClient, IServiceSettings serviceSettings)
    {
      InitializeComponent();
      this.carpetProcessor = carpetProcessor;
      this.cutRollList = cutRollList;
      this.mahloClient = mahloClient;
      this.serviceSettings = serviceSettings;
      this.statusBar1.StatusBarInfo = (IStatusBarInfo)this.carpetProcessor.PatternRepeatLogic;

      this.MahloPropertyChangedSubscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
        h => ((INotifyPropertyChanged)this.carpetProcessor.MahloLogic).PropertyChanged += h,
        h => ((INotifyPropertyChanged)this.carpetProcessor.MahloLogic).PropertyChanged -= h)
        .Where(args => args.EventArgs.PropertyName == nameof(this.carpetProcessor.MahloLogic.CurrentRoll))
        .Subscribe(args =>
        {
          this.mahloRollSrc.DataSource = this.carpetProcessor.MahloLogic.CurrentRoll;
          this.grdGreigeRoll.Invalidate();
        });

      BowAndSkewPropertyChangedSubscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
        h => ((INotifyPropertyChanged)this.carpetProcessor.BowAndSkewLogic).PropertyChanged += h,
        h => ((INotifyPropertyChanged)this.carpetProcessor.BowAndSkewLogic).PropertyChanged -= h)
        .Where(args => args.EventArgs.PropertyName == nameof(this.carpetProcessor.BowAndSkewLogic.CurrentRoll))
        .Subscribe(args =>
        {
          this.bowAndSkewRollSrc.DataSource = this.carpetProcessor.BowAndSkewLogic.CurrentRoll;
          this.grdGreigeRoll.Invalidate();
        });

      PatternRepeatPropertyChangedSubscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
        h => ((INotifyPropertyChanged)this.carpetProcessor.PatternRepeatLogic).PropertyChanged += h,
        h => ((INotifyPropertyChanged)this.carpetProcessor.PatternRepeatLogic).PropertyChanged -= h)
        .Where(args => args.EventArgs.PropertyName == nameof(this.carpetProcessor.PatternRepeatLogic.CurrentRoll))
        .Subscribe(args =>
        {
          this.patternRepeatRollSrc.DataSource = this.carpetProcessor.PatternRepeatLogic.CurrentRoll;
          this.grdGreigeRoll.Invalidate();
        });

      // Make column heading alignment match column data alignment
      foreach (DataGridViewColumn column in grdGreigeRoll.Columns)
      {
        column.HeaderCell.Style.Alignment = column.DefaultCellStyle.Alignment;
      }

      foreach (DataGridViewColumn column in grdCutRoll.Columns)
      {
        column.HeaderCell.Style.Alignment = column.DefaultCellStyle.Alignment;
      }

      this.colMalFeet.Tag =
        this.colBasFeet.Tag =
        this.colPrsFeet.Tag = new CellFormattingAction(this.SetFeetColor);

      this.colBow.Tag = new CellFormattingAction(this.SetBowColor);
      this.colSkew.Tag = new CellFormattingAction(this.SetSkewColor);
      this.colElongation.Tag = new CellFormattingAction(this.SetElongationColor);

      this.colCutBow.Tag = new CellFormattingAction(this.SetCutBowColor);
      this.colCutSkew.Tag = new CellFormattingAction(this.SetCutSkewColor);
      this.colCutEPE.Tag = new CellFormattingAction(this.SetCutEPEColor);
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
      this.grdGreigeRoll.DataSource = carpetProcessor.SewinQueue.Rolls;
      this.mahloRollSrc.DataSource = carpetProcessor.MahloLogic.CurrentRoll;
      this.bowAndSkewRollSrc.DataSource = carpetProcessor.BowAndSkewLogic.CurrentRoll;
      this.patternRepeatRollSrc.DataSource = carpetProcessor.PatternRepeatLogic.CurrentRoll;

      this.mahloLogicSrc.DataSource = carpetProcessor.MahloLogic;
      this.bowAndSkewLogicSrc.DataSource = carpetProcessor.BowAndSkewLogic;
      this.patternRepeatLogicSrc.DataSource = carpetProcessor.PatternRepeatLogic;
      this.cutRollSrc.DataSource = this.cutRollList;

      this.grpMahlo.Tag = nameof(IMahloLogic);
      this.grpBowAndSkew.Tag = nameof(IBowAndSkewLogic);
      this.grpPatternRepeat.Tag = nameof(IPatternRepeatLogic);

      this.grdGreigeRoll.ClearSelection();
    }

    private void GrdGreigeRoll_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      var col = this.grdGreigeRoll.Columns[e.ColumnIndex];
      e.Handled = string.IsNullOrWhiteSpace(col.DataPropertyName);
      if (e.Handled)
      {
        Rectangle rect = e.CellBounds;
        rect.X--;
        rect.Width++;
        e.Graphics.FillRectangle(SystemBrushes.AppWorkspace, rect);
      }
    }

    private void BtnFore_Click(object sender, EventArgs e)
    {
      this.ExecuteButtonCmd(sender, Ipc.MahloIpcClient.MoveToNextRollCommand);
    }

    private void BtnBack_Click(object sender, EventArgs e)
    {
      this.ExecuteButtonCmd(sender, Ipc.MahloIpcClient.MoveToPriorRollCommand);
    }

    private void BtnWaitForSeam_Click(object sender, EventArgs e)
    {
      this.ExecuteButtonCmd(sender, Ipc.MahloIpcClient.WaitForSeamCommand);
    }

    private async void ExecuteButtonCmd(object sender, string command)
    {
      Button button = (Button)sender;
      string name = (string)button.Parent.Tag;
      var buttons = button.Parent.Controls.OfType<Button>();
      buttons.ForEach(item => item.Enabled = false);
      await this.mahloClient.Call(command, name);
      buttons.ForEach(item => item.Enabled = true);
    }

    private void DataGridView1_SelectionChanged(object sender, EventArgs e)
    {
      // Prevent selection
      this.grdGreigeRoll.ClearSelection();
    }

    private void GrdGreigeRoll_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      var col = this.grdGreigeRoll.Columns[e.ColumnIndex];
      GreigeRoll gridRoll = carpetProcessor.SewinQueue.Rolls[e.RowIndex];
      SetColor(this.carpetProcessor.MahloLogic, mahloColumnNames);
      SetColor(this.carpetProcessor.BowAndSkewLogic, bowAndSkewColumnNames);
      SetColor(this.carpetProcessor.PatternRepeatLogic, patternRepeatColumnNames);

      void SetColor(IMeterLogic logic, string[] names)
      {
        if (names.Contains(col.DataPropertyName))
        {
          if (logic.CurrentRoll == gridRoll)
          {
            (e.CellStyle.BackColor, e.CellStyle.ForeColor) = CellColor.ActiveColor;
          }
          else if (e.RowIndex >= 0 && e.RowIndex < logic.CurrentRollIndex)
          {
            var action = this.grdGreigeRoll.Columns[e.ColumnIndex].Tag as CellFormattingAction;
            action?.Invoke(e, this.serviceSettings);
          }
        }
      }
    }

    private void GrdCutRoll_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      if (e.RowIndex >= 0)
      {
        var action = this.grdCutRoll.Columns[e.ColumnIndex].Tag as CellFormattingAction;
        action?.Invoke(e, this.serviceSettings);
      }
    }

    public void SetFeetColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = (GreigeRoll)this.grdGreigeRoll.Rows[args.RowIndex].DataBoundItem;
      long measuredLength = (long)args.Value;

      (args.CellStyle.BackColor, args.CellStyle.ForeColor) =
        roll.RollLength == 0 ?
        new CellColor { ForeColor = grdGreigeRoll.DefaultCellStyle.BackColor, BackColor = grdGreigeRoll.DefaultCellStyle.ForeColor } :
        CellColor.GetFeetColor(roll.RollLength, (long)args.Value, this.serviceSettings);
    }

    public void SetBowColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = (GreigeRoll)this.grdGreigeRoll.Rows[args.RowIndex].DataBoundItem;

      (args.CellStyle.BackColor, args.CellStyle.ForeColor) =
        CellColor.GetBowColor(roll.BackingCode, (double)args.Value, this.serviceSettings);
    }

    public void SetSkewColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = (GreigeRoll)this.grdGreigeRoll.Rows[args.RowIndex].DataBoundItem;

      (args.CellStyle.BackColor, args.CellStyle.ForeColor) =
        CellColor.GetSkewColor(roll.BackingCode, (double)args.Value, this.serviceSettings);
    }

    public void SetElongationColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {

    }

    public void SetCutBowColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = this.carpetProcessor.PatternRepeatLogic.CurrentRoll;

      (args.CellStyle.BackColor, args.CellStyle.ForeColor) =
        CellColor.GetBowColor(roll.BackingCode, (double)args.Value, this.serviceSettings);
    }

    public void SetCutSkewColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = this.carpetProcessor.PatternRepeatLogic.CurrentRoll;

      (args.CellStyle.BackColor, args.CellStyle.ForeColor) =
        CellColor.GetSkewColor(roll.BackingCode, (double)args.Value, this.serviceSettings);
    }

    public void SetCutEPEColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      
    }
  }
}
